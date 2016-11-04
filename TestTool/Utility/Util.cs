using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace VTSCore
{
	static class TaskExtensions
	{
		public static async void IgnorCompletion(this Task task)
		{
			await task;
		}
	}

	public static class Util
	{
		public static double Range(double value, double min, double max)
		{
			value = Math.Max(value, min);
			value = Math.Min(value, max);
			return value;
		}

		public static double GetDistance(Point p1, Point p2)
		{
			return GetDistance(p1.X, p1.Y, p2.X, p2.Y);
		}

		public static double GetDistance(double x1, double y1, double x2, double y2)
		{
			var dx = x1 - x2;
			var dy = y1 - y2;

			return Math.Sqrt(dx * dx + dy * dy);
		}

		public static void Subscribe<T>(this IObservable<T> source, Action<T> handler, CompositeDisposable disposeHandler)
		{
			disposeHandler.Add(source.Subscribe(handler));
		}

		public static void CopyPropertyValues(object source, object destination)
		{
			var query = from sp in source.GetType().GetProperties()
						from dp in destination.GetType().GetProperties()
						where (sp.Name == dp.Name) && dp.CanWrite && (dp.PropertyType == sp.PropertyType)
						select new { Proper = dp, Value = sp.GetValue(source, null) };

			foreach (var item in query)
			{
				item.Proper.SetValue(destination, item.Value, null);
			}
		}

		public static TValue Find<Tkey, TValue>(this IDictionary<Tkey, TValue> dic, Tkey key, TValue defaultValue)
		{
			TValue value = default(TValue);
			if (dic.TryGetValue(key, out value))
				return value;
			else
				return defaultValue;
		}

		public static TValue Find<Tkey, TValue>(this IDictionary<Tkey, TValue> dic, Tkey key)
		{
			return dic.Find(key, default(TValue));
		}

		public static bool IsSubClassOf(Type type, Type baseType)
		{
			var b = type.BaseType;
			while (b != null)
			{
				if (b.Equals(baseType))
				{
					return true;
				}
				b = b.BaseType;
			}
			return false;
		}

		public static void CreateAllCommands(Type type)
		{
			foreach (var proper in type.GetProperties())
			{
				proper.SetValue(null, Activator.CreateInstance(proper.PropertyType));
			}
		}

		public static string GetEnumDescription<T>(this T enumerationValue)
			where T : struct
		{
			var attr = GetEnumDesctiption<T, DescriptionAttribute>(enumerationValue);
			return attr == null ? null : attr.Description;
		}

		public static TAttribute GetEnumDesctiption<TEnum, TAttribute>(TEnum enumerationValue)
			where TEnum : struct
			where TAttribute : Attribute
		{
			var type = enumerationValue.GetType();
			var memberInfo = type.GetMember(enumerationValue.ToString()).First();
			return memberInfo.GetCustomAttributes(typeof(TAttribute), false)
							.FirstOrDefault() as TAttribute;
		}
	}

	public static class MefUtil
	{
		public static AggregateCatalog Catalog { get; private set; }

		static MefUtil()
		{
			var ass1 = System.Reflection.Assembly.GetEntryAssembly();
			var ass2 = typeof(MefUtil).Assembly;

			Catalog = new AggregateCatalog(new AssemblyCatalog(ass1), new AssemblyCatalog(ass2));
		}

		public static void GetExportedValue<T>(out T value)
		{
			value = GetExportedValue<T>();
		}

		public static void GetExportedValue<T>(string contractName, out T value)
		{
			value = GetExportedValue<T>(contractName);
		}

		public static T GetExportedValue<T>()
		{
			using (var container = new CompositionContainer(Catalog))
			{
				return container.GetExportedValue<T>();
			}
		}

		public static T GetExportedValue<T>(string contractName)
		{
			using (var container = new CompositionContainer(Catalog))
			{
				return container.GetExportedValue<T>(contractName);
			}
		}

		public static IEnumerable<T> GetExportedValues<T>()
		{
			using (var container = new CompositionContainer(Catalog))
			{
				return container.GetExportedValues<T>();
			}
		}

		public static IEnumerable<T> GetExportedValues<T>(string contractName)
		{
			using (var container = new CompositionContainer(Catalog))
			{
				return container.GetExportedValues<T>(contractName);
			}
		}

		public static void ComposeParts(params object[] objects)
		{
			using (var container = new CompositionContainer(Catalog))
			{
				container.ComposeParts(objects);
			}
		}
	}

	public static class WindowUtil
	{
		public static bool IsDesingMode()
		{
			return (bool)System.ComponentModel.DesignerProperties
						.IsInDesignModeProperty
						.GetMetadata(typeof(DependencyObject))
						.DefaultValue;
		}

		public static T FindLogicalParent<T>(this DependencyObject child) where T : class
		{
			var parent = LogicalTreeHelper.GetParent(child) as DependencyObject;

			if (parent == null)
				return null;
			else if (parent is T)
				return parent as T;
			else
				return FindLogicalParent<T>(parent);
		}

		public static T FindVisualParent<T>(this DependencyObject child) where T : class
		{
			var parent = VisualTreeHelper.GetParent(child);

			if (parent == null)
				return null;
			else if (parent is T)
				return parent as T;
			else
				return FindVisualParent<T>(parent);
		}

		public static IObservable<IObservable<Point>> GetDragDropEventSource(this FrameworkElement control, MouseButton triggerButton)
		{
			// 鼠标移动，取得坐标
			var mouseMove = from evt in Observable.FromEventPattern<MouseEventArgs>(control, "MouseMove")
							 select evt.EventArgs.GetPosition(control);

			// 鼠标在Shape上按下，开始DragDrop  
			var mouseDown = from evt in Observable.FromEventPattern<MouseButtonEventArgs>(control, "MouseDown")
							where evt.EventArgs.ChangedButton == triggerButton
							select evt.EventArgs.GetPosition(control);

			// 鼠标放开，终止DragDrop  
			var mouseUp = from evt in Observable.FromEventPattern<MouseButtonEventArgs>(control, "MouseUp")
						  where evt.EventArgs.ChangedButton == triggerButton
						  select evt.EventArgs.GetPosition(control);

			var dragDrop = from down in mouseDown
						   select mouseMove.TakeUntil(mouseUp);

			return dragDrop;
		}


		public static void AddCommandBinding(this UIElement c, ICommand cmd, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecuted = null)
		{
			canExecuted = canExecuted ?? new CanExecuteRoutedEventHandler((_1, e) => e.CanExecute = true);
			var cmdBinding = new CommandBinding(cmd, executed, canExecuted);
			c.CommandBindings.Add(cmdBinding);
		}
	}

	public static class ReactivieExteinsion
	{
		public static IObservable<T> DelayOne<T>(this IObservable<T> source)
		{
			return source.Zip(source.Skip(1), (i1, i2) => i1);
		}

		//[Obsolete]
		//public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> handler)
		//{
		//	return null;
		//}
	}

	public static class Contracts
	{
		public const string Initial = "Contracts.Initial";
	}
}

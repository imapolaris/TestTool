using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace VTSCore.Utility
{
	/// <summary>
	/// 主要用于某种类型的Adorner只有一个的情况
	/// </summary>
	class AdornerManager<TAdorner> where TAdorner : Adorner
	{
		UIElement element;
		private AdornerManager(UIElement element)
		{
			this.element = element;
		}

		protected virtual TAdorner CreateAdorner(UIElement element, params object[] para)
		{
			var type = typeof(TAdorner);
			var args = new[] { element }.Concat(para).ToArray();
			return type.Assembly.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, args, null, null) as TAdorner;
		}

		public bool HasAdorner { get { return Adorner != null; } }

		public TAdorner Adorner
		{
			get
			{
				var adornerLayer = AdornerLayer.GetAdornerLayer(element);
				if (adornerLayer == null)
					return null;

				var adorners = adornerLayer.GetAdorners(element);
				if (adorners == null)
					return null;

				return adorners.OfType<TAdorner>().FirstOrDefault();
			}
		}

		public void AddAdorner(params object[] para)
		{
			if (HasAdorner)
				return;

			var adornerLayer = AdornerLayer.GetAdornerLayer(element);
			adornerLayer.Add(CreateAdorner(element, para));
		}

		public void RemoveAdorner()
		{
			var adornerLayer = AdornerLayer.GetAdornerLayer(element);
			if (adornerLayer == null)
				return;

			var adorner = this.Adorner;
			if (adorner == null)
				return;

			adornerLayer.Remove(adorner);
		}

		public static AdornerManager<TAdorner> GetManager(UIElement element)
		{
			return new AdornerManager<TAdorner>(element);
		}
	}

	public static class AdornerHelper
	{
		public static void Remove<TAdorner>(UIElement element) where TAdorner : Adorner
		{
			AdornerManager<TAdorner>.GetManager(element).RemoveAdorner();
		}

		public static void Remove(Adorner adorner)
		{
			if (adorner == null)
				return;

			var layer = AdornerLayer.GetAdornerLayer(adorner.AdornedElement);
			layer.Remove(adorner);
		}

		public static TAdorner Add<TAdorner>(UIElement element, params object[] para) where TAdorner : Adorner
		{
			var manager = AdornerManager<TAdorner>.GetManager(element);
			manager.AddAdorner(para);
			return manager.Adorner;
		}

		public static TAdorner Get<TAdorner>(UIElement element) where TAdorner : Adorner
		{
			return AdornerManager<TAdorner>.GetManager(element).Adorner;
		}
	}
}

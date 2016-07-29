using System;
using System.Web.Http.ModelBinding;

namespace WebRemote
{
	public class DynamicModelBinderProvider : ModelBinderProvider
	{
		public override IModelBinder GetBinder(System.Web.Http.HttpConfiguration configuration, Type modelType)
		{
			return new DynamicModelBinder();
		}
	}
}
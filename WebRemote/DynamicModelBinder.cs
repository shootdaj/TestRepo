using System.Collections.Generic;
using System.Dynamic;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using ZoneLighting;

namespace WebRemote
{
	public class DynamicModelBinder : IModelBinder
	{
		//protected override object CreateModel(HttpActionContext controllerContext, ModelBindingContext bindingContext, Type modelType)
		//{
		//	if (modelType == typeof(Tuple<ContactModel, CommunicationModel, AddressModel>))
		//		return new Tuple<ContactModel, CommunicationModel, AddressModel>(new ContactModel(), new CommunicationModel(), new AddressModel());
		//	if (modelType == typeof(Tuple<ContactModel, CommunicationModel, AddressModel, CustomerModel>))
		//		return new Tuple<ContactModel, CommunicationModel, AddressModel, CustomerModel>(new ContactModel(), new CommunicationModel(), new AddressModel(), new CustomerModel());

		//	return base.CreateModel(controllerContext, bindingContext, modelType);
		//}


		public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
		{
			var json = actionContext.Request.Content.ReadAsStringAsync().Result;
			dynamic jsonDynamic = JsonConvert.DeserializeObject(json, new JsonSerializerSettings()
			{
				Converters = new List<JsonConverter>() {new ISVConverter()}
			});

			bindingContext.Model = jsonDynamic;
			//dynamic returnModel = new ExpandoObject();
			//var returnModelDictionary = returnModel as IDictionary<string, object>;
			//returnModelDictionary.Add("Item1", jsonDynamic[0]);
			//returnModelDictionary.Add("Item2", jsonDynamic[0]);

			return true;
		}
	}
}
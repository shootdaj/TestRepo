using System.Web.Mvc;
using System.Web.Mvc.Html;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Helpers
{
	public static class RazorHelper
	{
		public static string GetProgramActionMarkup(ProgramSet programSet)
		{
			var returnValue = MvcHtmlString.Create((programSet.State == ProgramState.Stopped
				? string.Format(
					"<input type='button' onclick=\"zlmCommand('start', '{0}', document.getElementById('ddl{0}Program').options[document.getElementById('ddl{0}Program').selectedIndex].text)\" value='Start' class='form-control btn btn-primary inTable'/>",
					programSet.Name)
				: string.Format(
					"<input type='button' onclick=\"zlmCommand('stop', '{0}')\" value='Stop' class='form-control btn btn-primary inTable'/>",
					programSet.Name)) +
				  string.Format(
					  "<span id='{0}Loading' style='display: none; text - align: right;'><img src='/WebController/Content/loadng.gif' alt = 'Busy...' width = '30px' height = '30px'></span>",
					  programSet.Name));

			return returnValue.ToHtmlString();
		}

		public static string GetProgramNameMarkup(this HtmlHelper helper, ProgramSet programSet, SelectList programList, ZoneProgram selectedProgram)
		{
			var returnValue = MvcHtmlString.Create(programSet.State == ProgramState.Stopped
				? helper.DropDownList("ddl" + programSet.Name + "Program", programList, new {@class = "form-control", style= "display: inline-flex; width: inherit;" }).ToString() + helper.Partial("ProgramSetInput", selectedProgram)
				: programSet.ProgramName);

			return returnValue.ToHtmlString();
		}
	}
}
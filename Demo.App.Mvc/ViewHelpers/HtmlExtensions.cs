using System.Web.Mvc;

namespace Demo.App.Mvc.ViewHelpers
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString ActionButton(this HtmlHelper helper, string value,
                              string action, string controller, object routeValues)
        {
            var a = (new UrlHelper(helper.ViewContext.RequestContext))
                                    .Action(action, controller, routeValues);

            var form = new TagBuilder("form");
            form.Attributes.Add("method", "get");
            form.Attributes.Add("action", a);

            var input = new TagBuilder("input");
            input.Attributes.Add("type", "submit");
            input.Attributes.Add("value", value);

            form.InnerHtml = input.ToString(TagRenderMode.SelfClosing);

            return MvcHtmlString.Create(form.ToString(TagRenderMode.Normal));
        }
    }
}
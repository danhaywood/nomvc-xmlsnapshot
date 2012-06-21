<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.WithServices.Master" Inherits="System.Web.Mvc.ViewPage<Demo.Dom.Claims.RecordedActionForClaim>" %>
<%@ Import namespace="Demo.App.Mvc.ViewHelpers" %>
<%@ Import Namespace="NakedObjects.Resources" %>
<%@ Import Namespace="NakedObjects.Web.Mvc.Html"%>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%:  Html.ObjectTitle(Model)%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
.link-button {
    -moz-border-radius:0.333em 0.333em 0.333em 0.333em;
    -moz-box-shadow:0 1px 4px rgba(0, 0, 0, 0.4);
    background:-moz-linear-gradient(center top , white, #306AB5 4%, #274976) repeat scroll 0 0 transparent;
    border-color:#306AB5 #2B5892 #274771;
    border-style:solid;
    border-width:1px;
    color:white;
    cursor:pointer;
    display:inline-block;
    font-size:1.167em;
    font-weight:bold;
    line-height:1.429em;
    padding:0.286em 1em 0.357em;
}        
    </style>
    <%: Html.History(Model)%>   
    <div class="<%: IdHelper.ObjectViewName %>" id="<%: Html.ObjectTypeAsCssId(Model) %>">
        
        <%: Html.Object(Model)%>
        <%: Html.UserMessages() %>

        <%: Html.Menu(Model)%>
        <%if (Html.ObjectHasVisibleFields(Model)) {%>
            <%: Html.PropertyListWithout(Model, ra => ra.XmlSnapshot)%>   
        <%}%>

        <%: Html.ActionLink("View Snapshot", "ViewSnapshot", "RecordedAction", new { id =Model.Id}, new { @class = "link-button" } )%>
        
    </div>
</asp:Content>

<%@ Master Language="C#" Inherits="System.Web.Mvc.ViewMasterPage" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title><asp:ContentPlaceHolder ID="TitleContent" runat="server" /></title>
    <link href="../../Content/NakedObjects.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.6.min.js"></script>
</head>
<body>
    <div class="page">
        <div id="header">
            <div id="title">
                <h1>Naked Objects MVC</h1>
            </div>
        </div>
        <div id="main">
            <asp:ContentPlaceHolder ID="MainContent" runat="server" />
            <div id="footer">
            </div>
        </div>
    </div>
</body>
</html>

﻿@* To use Razor views, see Naked Objects Manual (search for 'Razor') *@
@model $rootnamespace$.Models.ChangePasswordModel

@{
	Layout = "Site.Account.cshtml";
    ViewBag.Title = "Change Password";
}

<h2>Change Password</h2>
<p>
    Use the form below to change your password. 
</p>
<p>
    New passwords are required to be a minimum of @Membership.MinRequiredPasswordLength characters in length.
</p>

    <script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jQuery.Validate/1.8.1/jQuery.Validate.min.js"></script>
    <script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.validate.unobtrusive.min.js"></script>

@using (Html.BeginForm()) {
    @Html.ValidationSummary(true, "Password change was unsuccessful. Please correct the errors and try again.")
    <div>
        <fieldset>
            <legend>Account Information</legend>

            <div class="editor-label">
                @Html.LabelFor(m => m.OldPassword)
            </div>
            <div class="editor-field">
                @Html.PasswordFor(m => m.OldPassword)
                @Html.ValidationMessageFor(m => m.OldPassword)
            </div>

            <div class="editor-label">
                @Html.LabelFor(m => m.NewPassword)
            </div>
            <div class="editor-field">
                @Html.PasswordFor(m => m.NewPassword)
                @Html.ValidationMessageFor(m => m.NewPassword)
            </div>

            <div class="editor-label">
                @Html.LabelFor(m => m.ConfirmPassword)
            </div>
            <div class="editor-field">
                @Html.PasswordFor(m => m.ConfirmPassword)
                @Html.ValidationMessageFor(m => m.ConfirmPassword)
            </div>

            <p>
                <input type="submit" value="Change Password" />
            </p>
        </fieldset>
    </div>
}

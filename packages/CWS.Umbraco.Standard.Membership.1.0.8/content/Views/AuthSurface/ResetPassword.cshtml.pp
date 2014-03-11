﻿@using System.Web.Mvc
@using System.Web.Mvc.Html
@using Umbraco.Web
@using $rootnamespace$.Models
@using $rootnamespace$.Controllers.SurfaceControllers
@model ResetPasswordViewModel

@{
    Html.EnableClientValidation(true);
    Html.EnableUnobtrusiveJavaScript(true);
}

@if (!ViewData.ModelState.IsValid)
{

    <h3>Errors</h3>
    foreach (ModelState modelState in ViewData.ModelState.Values)
    {
        var errors = modelState.Errors;

        if (errors.Any())
        { 
            <ul>
                @foreach(ModelError error in errors)
                {
                    <li><em>@error.ErrorMessage</em></li>
                }
            </ul>
        }
    }
}

@using(Html.BeginUmbracoForm<AuthSurfaceController>("HandleResetPassword") )
{
    @Html.ValidationSummary(true)

    <fieldset>
        <legend>Reset your Password</legend>
        
		<div class="editor-label">
            @Html.LabelFor(model => model.EmailAddress)
        </div>

        <div class="editor-field">
            @Html.EditorFor(model => model.EmailAddress)
            @Html.ValidationMessageFor(model => model.EmailAddress)
        </div>
        
        <div class="editor-label">
            @Html.LabelFor(model => model.Password)
        </div>

        <div class="editor-field">
            @Html.EditorFor(model => model.Password)
            @Html.ValidationMessageFor(model => model.Password)
        </div>
        
        <div class="editor-label">
            @Html.LabelFor(model => model.ConfirmPassword)
        </div>

        <div class="editor-field">
            @Html.EditorFor(model => model.ConfirmPassword)
            @Html.ValidationMessageFor(model => model.ConfirmPassword)
        </div>
        
        <p>
            <input type="submit" value="Reset Password" />
        </p>
    </fieldset>
}
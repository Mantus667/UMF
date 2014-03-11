@using System.Web.Mvc.Html
@using Umbraco.Web
@using $rootnamespace$.Controllers.SurfaceControllers
@using $rootnamespace$.Models
@using umbraco.cms.businesslogic.member
@model RegisterViewModel

@{
    Html.EnableClientValidation(true);
    Html.EnableUnobtrusiveJavaScript(true);
}

@using(Html.BeginUmbracoForm<AuthSurfaceController>("HandleRegister") )
{
    @Html.ValidationSummary(true)

    <fieldset>
        <legend>Register</legend>
        
        <div class="editor-label">
            @Html.LabelFor(model => model.Name)
        </div>

        <div class="editor-field">
            @Html.EditorFor(model => model.Name)
            @Html.ValidationMessageFor(model => model.Name)
        </div>

        
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
            <input type="submit" value="Register" />
        </p>
    </fieldset>
}
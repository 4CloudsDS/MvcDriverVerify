#pragma checksum "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "87c2f0b8f629516e403cb78bdb7e92c239253668"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Comments_Details), @"mvc.1.0.view", @"/Views/Comments/Details.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Comments/Details.cshtml", typeof(AspNetCore.Views_Comments_Details))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "D:\www\VerifyDriver\MvcDriverVerify\Views\_ViewImports.cshtml"
using MvcDriverVerify;

#line default
#line hidden
#line 2 "D:\www\VerifyDriver\MvcDriverVerify\Views\_ViewImports.cshtml"
using MvcDriverVerify.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"87c2f0b8f629516e403cb78bdb7e92c239253668", @"/Views/Comments/Details.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"664c8a18748fc37bf757d20525431d1694950c42", @"/Views/_ViewImports.cshtml")]
    public class Views_Comments_Details : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MvcDriverVerify.Models.Comments>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Edit", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(40, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
  
    ViewData["Title"] = "Details";

#line default
#line hidden
            BeginContext(85, 131, true);
            WriteLiteral("\r\n<h1>Details</h1>\r\n\r\n<div>\r\n    <h4>Comments</h4>\r\n    <hr />\r\n    <dl class=\"row\">\r\n        <dt class = \"col-sm-4\">\r\n            ");
            EndContext();
            BeginContext(217, 41, false);
#line 14 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.CText));

#line default
#line hidden
            EndContext();
            BeginContext(258, 62, true);
            WriteLiteral("\r\n        </dt>\r\n        <dd class = \"col-sm-5\">\r\n            ");
            EndContext();
            BeginContext(321, 37, false);
#line 17 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
       Write(Html.DisplayFor(model => model.CText));

#line default
#line hidden
            EndContext();
            BeginContext(358, 62, true);
            WriteLiteral("\r\n        </dd>\r\n        <dt class = \"col-sm-4\">\r\n            ");
            EndContext();
            BeginContext(421, 45, false);
#line 20 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.CDateTime));

#line default
#line hidden
            EndContext();
            BeginContext(466, 62, true);
            WriteLiteral("\r\n        </dt>\r\n        <dd class = \"col-sm-5\">\r\n            ");
            EndContext();
            BeginContext(529, 41, false);
#line 23 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
       Write(Html.DisplayFor(model => model.CDateTime));

#line default
#line hidden
            EndContext();
            BeginContext(570, 62, true);
            WriteLiteral("\r\n        </dd>\r\n        <dt class = \"col-sm-4\">\r\n            ");
            EndContext();
            BeginContext(633, 38, false);
#line 26 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.CP));

#line default
#line hidden
            EndContext();
            BeginContext(671, 62, true);
            WriteLiteral("\r\n        </dt>\r\n        <dd class = \"col-sm-5\">\r\n            ");
            EndContext();
            BeginContext(734, 41, false);
#line 29 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
       Write(Html.DisplayFor(model => model.CP.UNames));

#line default
#line hidden
            EndContext();
            BeginContext(775, 62, true);
            WriteLiteral("\r\n        </dd>\r\n        <dt class = \"col-sm-4\">\r\n            ");
            EndContext();
            BeginContext(838, 38, false);
#line 32 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.CU));

#line default
#line hidden
            EndContext();
            BeginContext(876, 62, true);
            WriteLiteral("\r\n        </dt>\r\n        <dd class = \"col-sm-5\">\r\n            ");
            EndContext();
            BeginContext(939, 41, false);
#line 35 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
       Write(Html.DisplayFor(model => model.CU.UNames));

#line default
#line hidden
            EndContext();
            BeginContext(980, 47, true);
            WriteLiteral("\r\n        </dd>\r\n    </dl>\r\n</div>\r\n<div>\r\n    ");
            EndContext();
            BeginContext(1027, 55, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "87c2f0b8f629516e403cb78bdb7e92c2392536687425", async() => {
                BeginContext(1074, 4, true);
                WriteLiteral("Edit");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#line 40 "D:\www\VerifyDriver\MvcDriverVerify\Views\Comments\Details.cshtml"
                           WriteLiteral(Model.CId);

#line default
#line hidden
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1082, 8, true);
            WriteLiteral(" |\r\n    ");
            EndContext();
            BeginContext(1090, 38, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "87c2f0b8f629516e403cb78bdb7e92c2392536689733", async() => {
                BeginContext(1112, 12, true);
                WriteLiteral("Back to List");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1128, 10, true);
            WriteLiteral("\r\n</div>\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MvcDriverVerify.Models.Comments> Html { get; private set; }
    }
}
#pragma warning restore 1591

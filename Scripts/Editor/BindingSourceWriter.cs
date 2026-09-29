using System.Text;

namespace MvvmUnity.Editor
{
    internal static class BindingSourceWriter
    {
        public const string OwnershipMarker = "GeneratedCode(\"MvvmUnity.BindingCodeGenerator\"";

        private const string GeneratedCodeAttribute =
            "[global::System.CodeDom.Compiler." + OwnershipMarker + ", \"2.0\")]";

        public static string Write(ViewBindingPlan plan)
        {
            var code = new StringBuilder();
            var viewType = plan.ViewType;
            var hasNamespace = !string.IsNullOrEmpty(viewType.Namespace);
            var indent = hasNamespace ? "    " : string.Empty;

            Line(code, string.Empty, "using MvvmUnity.Unity;");
            Line(code, string.Empty, string.Empty);
            if (hasNamespace)
            {
                Line(code, string.Empty, "namespace " + viewType.Namespace);
                Line(code, string.Empty, "{");
            }

            Line(code, indent, (viewType.IsPublic ? "public" : "internal") + " partial class " + viewType.Name);
            Line(code, indent, "{");
            Line(code, indent, "    " + GeneratedCodeAttribute);
            Line(code, indent, "    protected override void BindGenerated(");
            Line(code, indent, "        global::MvvmUnity.Unity.BindingScope bindings,");
            Line(code, indent, "        " + TypeNames.Qualified(plan.ViewModelType) + " viewModel)");
            Line(code, indent, "    {");
            foreach (var field in plan.Fields)
                Line(code, indent, $"        bindings.{field.Target}({field.Field}, viewModel.{field.Source});");
            foreach (var observer in plan.Observers)
                Line(code, indent, $"        bindings.Observe(viewModel.{observer.Source}, {observer.Method});");
            Line(code, indent, "    }");
            Line(code, indent, "}");
            if (hasNamespace)
                Line(code, string.Empty, "}");

            return code.ToString();
        }

        private static void Line(StringBuilder code, string indent, string text)
        {
            if (text.Length > 0)
                code.Append(indent).Append(text);

            code.Append('\n');
        }
    }
}

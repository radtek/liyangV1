using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace WCAPP
{
    public static class Exends
    {
        public static string ToShortDateString(this DateTime? val)
        {
            if (val.HasValue)
            {
                return val.Value.ToShortDateString();
            }

            return "";
        }

        public static string ToString<T>(this T? val, string nilStr) where T : struct
        {
            if (val.HasValue)
            {
                return val.Value.ToString();
            }

            return nilStr;
        }

        public static string ToString(this string val, string nilStr)
        {
            if (val != null)
            {
                return val;
            }

            return nilStr;
        }

        public static string ToString<T>(this T? val, string nilStr, Func<T, string> func) where T : struct
        {
            if (val.HasValue)
            {
                return func(val.Value);
            }

            return nilStr;
        }

        public static string ToString<T>(this T? val, Func<T, string> func) where T : struct
        {
            return ToString(val, null, func);
        }

        public static bool NotEmpty<T>(this IEnumerable<T> list)
        {
            return list != null && list.Any();
        }

        public static string ToString<T>(this IEnumerable<T> list, string sepor, string nilStr, Func<T, string> func,
            string ignore = null)
        {
            if (list == null)
                return nilStr;

            string str = "";
            bool first = true;

            foreach (var v in list)
            {
                string s = func(v);
                if (s == null || s == ignore)
                    continue;

                if (first)
                {
                    str = s;
                    first = false;
                }
                else
                {
                    str += sepor + s;
                }
            }

            return str;
        }

        public static string ToString<T>(this IEnumerable<T> list, string sepor, Func<T, string> func,
            string ignore = null)
        {
            return ToString(list, sepor, null, func, ignore);
        }

        public static string ToStringUniq<T>(this IEnumerable<T> list, string sepor, string nilStr,
            Func<T, string> func, string ignore = null)
        {
            if (list == null)
                return nilStr;

            string str = "";
            bool first = true;
            HashSet<string> set = new HashSet<string>();

            foreach (var v in list)
            {
                string s = func(v);

                if (s == null || s == ignore)
                    continue;

                if (set.Contains(s))
                    continue;

                set.Add(s);

                if (first)
                {
                    str = s;
                    first = false;
                }
                else
                {
                    str += sepor + s;
                }
            }

            return str;
        }

        public static string ToStringUniq<T>(this IEnumerable<T> list, string sepor, Func<T, string> func,
            string ignore = null)
        {
            return ToStringUniq(list, sepor, null, func, ignore);
        }

        public static HtmlString FillHtml<T>(this IEnumerable<T> list, string fmt, Func<T, string> func)
        {
            if (list == null)
                return null;

            string html = "";
            foreach (var v in list)
            {
                string s = func(v);
                if (s == null)
                    continue;

                html += String.Format(fmt, s);
            }

            return new HtmlString(html);
        }

        public static MvcHtmlString InputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty?>> expression) where TProperty : struct
        {
            MvcHtmlString html;

            if (typeof(TProperty).IsEnum)
                html = htmlHelper.DropDownListFor(expression, Enum<TProperty>.GetSelectList(), "-- 请选择 --");
            else
                html = htmlHelper.TextBoxFor(expression);

            return new MvcHtmlString(html + htmlHelper.ValidationMessageFor(expression).ToString());
        }

        public static MvcHtmlString Input<TModel>(this HtmlHelper<TModel> htmlHelper, string name, Type type)
        {
            MvcHtmlString html;

            if (type.IsEnum)
                html = htmlHelper.DropDownList(name, new SelectList(Enum.GetValues(type)), "-- 请选择 --");
            else
                html = htmlHelper.TextBox(name);

            return html;
        }

        public static MvcHtmlString Input<TModel>(this HtmlHelper<TModel> htmlHelper, string name)
        {
            return htmlHelper.TextBox(name);
        }

        public static MvcHtmlString InputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            MvcHtmlString html;

            if (typeof(TProperty).IsEnum)
                html = htmlHelper.DropDownListFor(expression, Enum<TProperty>.GetSelectList(), "-- 请选择 --");
            else
                html = htmlHelper.TextBoxFor(expression);

            return new MvcHtmlString(html + htmlHelper.ValidationMessageFor(expression).ToString());
        }

        public static MvcHtmlString SelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TProperty:struct
        {
            return htmlHelper.DropDownListFor(expression, Enum<TProperty>.GetSelectList(), "-- 请选择 --");
        }

        public static MvcHtmlString SelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAtrribute) where TProperty:struct
        {
            return htmlHelper.DropDownListFor(expression, Enum<TProperty>.GetSelectList(), "-- 请选择 --", htmlAtrribute);
        }

        public static MvcHtmlString SelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty?>> expression) where TProperty:struct
        {
            return htmlHelper.DropDownListFor(expression, Enum<TProperty>.GetSelectList(), "-- 请选择 --");
        }

        public static MvcHtmlString SelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty?>> expression, object htmlAtrribute) where TProperty:struct
        {
            return htmlHelper.DropDownListFor(expression, Enum<TProperty>.GetSelectList(), "-- 请选择 --", htmlAtrribute);
        }
    }
}
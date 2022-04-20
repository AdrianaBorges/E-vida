
/* French initialisation for the jQuery UI date picker plugin. */
/* Written by Keith Wood (kbwood@virginbroadband.com.au) and Stéphane Nahmani (sholby@sholby.net). */
jQuery(function ($) {
    $.datepicker.regional['pt'] = { 
        closeText: "Fechar", prevText: "Anterior", nextText: "Próximo", currentText: "Hoje",
        monthNames: ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"],
        monthNamesShort: ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
        dayNames: ["Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"],
        dayNamesShort: ["Dom", "Mon", "Ter", "Qua", "Qui", "Sex", "Sáb"],
        dayNamesMin: ["D", "M", "T", "Q", "Q", "S", "S"],
        weekHeader: "Sem",
        dateFormat: "dd/mm/yy",
        firstDay: 0,
        initStatus: 'Escolha a data', isRTL: false
    };
    $.datepicker.setDefaults($.datepicker.regional['pt']);
});


/*
Masked Input plugin for jQuery
Copyright (c) 2007-2013 Josh Bush (digitalbush.com)
Licensed under the MIT license (http://digitalbush.com/projects/masked-input-plugin/#license)
Version: 1.3.1
*/
(function (e) { function t() { var e = document.createElement("input"), t = "onpaste"; return e.setAttribute(t, ""), "function" == typeof e[t] ? "paste" : "input" } var n, a = t() + ".mask", r = navigator.userAgent, i = /iphone/i.test(r), o = /android/i.test(r); e.mask = { definitions: { 9: "[0-9]", a: "[A-Za-z]", "*": "[A-Za-z0-9]" }, dataName: "rawMaskFn", placeholder: "_" }, e.fn.extend({ caret: function (e, t) { var n; if (0 !== this.length && !this.is(":hidden")) return "number" == typeof e ? (t = "number" == typeof t ? t : e, this.each(function () { this.setSelectionRange ? this.setSelectionRange(e, t) : this.createTextRange && (n = this.createTextRange(), n.collapse(!0), n.moveEnd("character", t), n.moveStart("character", e), n.select()) })) : (this[0].setSelectionRange ? (e = this[0].selectionStart, t = this[0].selectionEnd) : document.selection && document.selection.createRange && (n = document.selection.createRange(), e = 0 - n.duplicate().moveStart("character", -1e5), t = e + n.text.length), { begin: e, end: t }) }, unmask: function () { return this.trigger("unmask") }, mask: function (t, r) { var c, l, s, u, f, h; return !t && this.length > 0 ? (c = e(this[0]), c.data(e.mask.dataName)()) : (r = e.extend({ placeholder: e.mask.placeholder, completed: null }, r), l = e.mask.definitions, s = [], u = h = t.length, f = null, e.each(t.split(""), function (e, t) { "?" == t ? (h--, u = e) : l[t] ? (s.push(RegExp(l[t])), null === f && (f = s.length - 1)) : s.push(null) }), this.trigger("unmask").each(function () { function c(e) { for (; h > ++e && !s[e]; ); return e } function d(e) { for (; --e >= 0 && !s[e]; ); return e } function m(e, t) { var n, a; if (!(0 > e)) { for (n = e, a = c(t); h > n; n++) if (s[n]) { if (!(h > a && s[n].test(R[a]))) break; R[n] = R[a], R[a] = r.placeholder, a = c(a) } b(), x.caret(Math.max(f, e)) } } function p(e) { var t, n, a, i; for (t = e, n = r.placeholder; h > t; t++) if (s[t]) { if (a = c(t), i = R[t], R[t] = n, !(h > a && s[a].test(i))) break; n = i } } function g(e) { var t, n, a, r = e.which; 8 === r || 46 === r || i && 127 === r ? (t = x.caret(), n = t.begin, a = t.end, 0 === a - n && (n = 46 !== r ? d(n) : a = c(n - 1), a = 46 === r ? c(a) : a), k(n, a), m(n, a - 1), e.preventDefault()) : 27 == r && (x.val(S), x.caret(0, y()), e.preventDefault()) } function v(t) { var n, a, i, l = t.which, u = x.caret(); t.ctrlKey || t.altKey || t.metaKey || 32 > l || l && (0 !== u.end - u.begin && (k(u.begin, u.end), m(u.begin, u.end - 1)), n = c(u.begin - 1), h > n && (a = String.fromCharCode(l), s[n].test(a) && (p(n), R[n] = a, b(), i = c(n), o ? setTimeout(e.proxy(e.fn.caret, x, i), 0) : x.caret(i), r.completed && i >= h && r.completed.call(x))), t.preventDefault()) } function k(e, t) { var n; for (n = e; t > n && h > n; n++) s[n] && (R[n] = r.placeholder) } function b() { x.val(R.join("")) } function y(e) { var t, n, a = x.val(), i = -1; for (t = 0, pos = 0; h > t; t++) if (s[t]) { for (R[t] = r.placeholder; pos++ < a.length; ) if (n = a.charAt(pos - 1), s[t].test(n)) { R[t] = n, i = t; break } if (pos > a.length) break } else R[t] === a.charAt(pos) && t !== u && (pos++, i = t); return e ? b() : u > i + 1 ? (x.val(""), k(0, h)) : (b(), x.val(x.val().substring(0, i + 1))), u ? t : f } var x = e(this), R = e.map(t.split(""), function (e) { return "?" != e ? l[e] ? r.placeholder : e : void 0 }), S = x.val(); x.data(e.mask.dataName, function () { return e.map(R, function (e, t) { return s[t] && e != r.placeholder ? e : null }).join("") }), x.attr("readonly") || x.one("unmask", function () { x.unbind(".mask").removeData(e.mask.dataName) }).bind("focus.mask", function () { clearTimeout(n); var e; S = x.val(), e = y(), n = setTimeout(function () { b(), e == t.length ? x.caret(0, e) : x.caret(e) }, 10) }).bind("blur.mask", function () { y(), x.val() != S && x.change() }).bind("keydown.mask", g).bind("keypress.mask", v).bind(a, function () { setTimeout(function () { var e = y(!0); x.caret(e), r.completed && e == x.val().length && r.completed.call(x) }, 0) }), y() })) } }) })(jQuery);


(function (d) {
    d.fn.priceFormat = function (a) {
        a = d.extend({ prefix: "US$ ", suffix: "", centsSeparator: ".", thousandsSeparator: ",", limit: !1, centsLimit: 2, clearPrefix: !1, clearSufix: !1, allowNegative: !1 }, a); return this.each(function () {
            function g(k) {
                for (var b = "", a = 0; a < k.length; a++) char_ = k.charAt(a), 0 == b.length && 0 == char_ && (char_ = !1), char_ && char_.match(t) && (m ? b.length < m && (b += char_) : b += char_); for (var c; b.length < j + 1; ) b = "0" + b; c = b; var b = "", a = 0, d = c.substr(c.length - j, j), e = c.substr(0, c.length - j); c = e + n + d; if (l) {
                    for (c = e.length; 0 <
c; c--) char_ = e.substr(c - 1, 1), a++, 0 == a % 3 && (char_ = l + char_), b = char_ + b; b.substr(0, 1) == l && (b = b.substring(1, b.length)); c = b + n + d
                } if (o && -1 != k.indexOf("-") && (0 != e || 0 != d)) c = "-" + c; h && (c = h + c); i && (c += i); return c
            } function f() { var a = e.val(), b = g(a); a != b && e.val(b) } function p() { if ("" != d.trim(h) && q) { var a = e.val().split(h); e.val(a[1]) } } function r() { if ("" != d.trim(i) && s) { var a = e.val().split(i); e.val(a[0]) } } var e = d(this), t = /[0-9]/, h = a.prefix, i = a.suffix, n = a.centsSeparator, l = a.thousandsSeparator, m = a.limit, j = a.centsLimit, q =
a.clearPrefix, s = a.clearSuffix, o = a.allowNegative; d(this).bind("keydown", function (a) { var b = a.keyCode ? a.keyCode : a.which, d = String.fromCharCode(b), c = !1, f = e.val(), d = g(f + d); if (48 <= b && 57 >= b || 96 <= b && 105 >= b) c = !0; 8 == b && (c = !0); 9 == b && (c = !0); 13 == b && (c = !0); 46 == b && (c = !0); 37 == b && (c = !0); 39 == b && (c = !0); if (o && (189 == b || 109 == b)) c = !0; c || (a.preventDefault(), a.stopPropagation(), f != d && e.val(d)) }); d(this).bind("keyup", f); q && (d(this).bind("focusout", function () { p() }), d(this).bind("focusin", function () { var a = e.val(); e.val(h + a) }));
            s && (d(this).bind("focusout", function () { r() }), d(this).bind("focusin", function () { var a = e.val(); e.val(a + i) })); 0 < d(this).val().length && (f(), p(), r())
        })
    }; jQuery.fn.unmask = function () { var a = d(this).val(), g = "", f; for (f in a) if (!isNaN(a[f]) || "-" == a[f]) g += a[f]; return g } 
})(jQuery);



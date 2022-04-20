
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

// jQuery Mask Plugin v1.13.4
// github.com/igorescobar/jQuery-Mask-Plugin
/*
(function (b) { "function" === typeof define && define.amd ? define(["jquery"], b) : "object" === typeof exports ? module.exports = b(require("jquery")) : b(jQuery || Zepto) })(function (b) {
    var y = function (a, c, d) {
        a = b(a); var g = this, k = a.val(), l; c = "function" === typeof c ? c(a.val(), void 0, a, d) : c; var e = {
            invalid: [], getCaret: function () {
                try {
                    var q, b = 0, e = a.get(0), f = document.selection, c = e.selectionStart; if (f && -1 === navigator.appVersion.indexOf("MSIE 10")) q = f.createRange(), q.moveStart("character", a.is("input") ? -a.val().length : -a.text().length),
                    b = q.text.length; else if (c || "0" === c) b = c; return b
                } catch (d) { }
            }, setCaret: function (q) { try { if (a.is(":focus")) { var b, c = a.get(0); c.setSelectionRange ? c.setSelectionRange(q, q) : c.createTextRange && (b = c.createTextRange(), b.collapse(!0), b.moveEnd("character", q), b.moveStart("character", q), b.select()) } } catch (f) { } }, events: function () {
                a.on("input.mask keyup.mask", e.behaviour).on("paste.mask drop.mask", function () { setTimeout(function () { a.keydown().keyup() }, 100) }).on("change.mask", function () { a.data("changed", !0) }).on("blur.mask",
                function () { k === a.val() || a.data("changed") || a.triggerHandler("change"); a.data("changed", !1) }).on("blur.mask", function () { k = a.val() }).on("focus.mask", function (a) { !0 === d.selectOnFocus && b(a.target).select() }).on("focusout.mask", function () { d.clearIfNotMatch && !l.test(e.val()) && e.val("") })
            }, getRegexMask: function () {
                for (var a = [], b, e, f, d, h = 0; h < c.length; h++) (b = g.translation[c.charAt(h)]) ? (e = b.pattern.toString().replace(/.{1}$|^.{1}/g, ""), f = b.optional, (b = b.recursive) ? (a.push(c.charAt(h)), d = {
                    digit: c.charAt(h),
                    pattern: e
                }) : a.push(f || b ? e + "?" : e)) : a.push(c.charAt(h).replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&")); a = a.join(""); d && (a = a.replace(new RegExp("(" + d.digit + "(.*" + d.digit + ")?)"), "($1)?").replace(new RegExp(d.digit, "g"), d.pattern)); return new RegExp(a)
            }, destroyEvents: function () { a.off("input keydown keyup paste drop blur focusout ".split(" ").join(".mask ")) }, val: function (b) { var c = a.is("input") ? "val" : "text"; if (0 < arguments.length) { if (a[c]() !== b) a[c](b); c = a } else c = a[c](); return c }, getMCharsBeforeCount: function (a,
            b) { for (var e = 0, f = 0, d = c.length; f < d && f < a; f++) g.translation[c.charAt(f)] || (a = b ? a + 1 : a, e++); return e }, caretPos: function (a, b, d, f) { return g.translation[c.charAt(Math.min(a - 1, c.length - 1))] ? Math.min(a + d - b - f, d) : e.caretPos(a + 1, b, d, f) }, behaviour: function (a) {
                a = a || window.event; e.invalid = []; var c = a.keyCode || a.which; if (-1 === b.inArray(c, g.byPassKeys)) {
                    var d = e.getCaret(), f = e.val().length, n = d < f, h = e.getMasked(), k = h.length, m = e.getMCharsBeforeCount(k - 1) - e.getMCharsBeforeCount(f - 1); e.val(h); !n || 65 === c && a.ctrlKey || (8 !==
                    c && 46 !== c && (d = e.caretPos(d, f, k, m)), e.setCaret(d)); return e.callbacks(a)
                }
            }, getMasked: function (a) {
                var b = [], k = e.val(), f = 0, n = c.length, h = 0, l = k.length, m = 1, p = "push", u = -1, t, w; d.reverse ? (p = "unshift", m = -1, t = 0, f = n - 1, h = l - 1, w = function () { return -1 < f && -1 < h }) : (t = n - 1, w = function () { return f < n && h < l }); for (; w() ;) {
                    var x = c.charAt(f), v = k.charAt(h), r = g.translation[x]; if (r) v.match(r.pattern) ? (b[p](v), r.recursive && (-1 === u ? u = f : f === t && (f = u - m), t === u && (f -= m)), f += m) : r.optional ? (f += m, h -= m) : r.fallback ? (b[p](r.fallback), f += m, h -= m) : e.invalid.push({
                        p: h,
                        v: v, e: r.pattern
                    }), h += m; else { if (!a) b[p](x); v === x && (h += m); f += m }
                } a = c.charAt(t); n !== l + 1 || g.translation[a] || b.push(a); return b.join("")
            }, callbacks: function (b) { var g = e.val(), l = g !== k, f = [g, b, a, d], n = function (a, b, c) { "function" === typeof d[a] && b && d[a].apply(this, c) }; n("onChange", !0 === l, f); n("onKeyPress", !0 === l, f); n("onComplete", g.length === c.length, f); n("onInvalid", 0 < e.invalid.length, [g, b, a, e.invalid, d]) }
        }; g.mask = c; g.options = d; g.remove = function () {
            var b = e.getCaret(); e.destroyEvents(); e.val(g.getCleanVal()); e.setCaret(b -
            e.getMCharsBeforeCount(b)); return a
        }; g.getCleanVal = function () { return e.getMasked(!0) }; g.init = function (c) {
            c = c || !1; d = d || {}; g.byPassKeys = b.jMaskGlobals.byPassKeys; g.translation = b.jMaskGlobals.translation; g.translation = b.extend({}, g.translation, d.translation); g = b.extend(!0, {}, g, d); l = e.getRegexMask(); !1 === c ? (d.placeholder && a.attr("placeholder", d.placeholder), b("input").length && !1 === "oninput" in b("input")[0] && "on" === a.attr("autocomplete") && a.attr("autocomplete", "off"), e.destroyEvents(), e.events(), c = e.getCaret(),
            e.val(e.getMasked()), e.setCaret(c + e.getMCharsBeforeCount(c, !0))) : (e.events(), e.val(e.getMasked()))
        }; g.init(!a.is("input"))
    }; b.maskWatchers = {}; var A = function () { var a = b(this), c = {}, d = a.attr("data-mask"); a.attr("data-mask-reverse") && (c.reverse = !0); a.attr("data-mask-clearifnotmatch") && (c.clearIfNotMatch = !0); "true" === a.attr("data-mask-selectonfocus") && (c.selectOnFocus = !0); if (z(a, d, c)) return a.data("mask", new y(this, d, c)) }, z = function (a, c, d) {
        d = d || {}; var g = b(a).data("mask"), k = JSON.stringify; a = b(a).val() ||
        b(a).text(); try { return "function" === typeof c && (c = c(a)), "object" !== typeof g || k(g.options) !== k(d) || g.mask !== c } catch (l) { }
    }; b.fn.mask = function (a, c) { c = c || {}; var d = this.selector, g = b.jMaskGlobals, k = b.jMaskGlobals.watchInterval, l = function () { if (z(this, a, c)) return b(this).data("mask", new y(this, a, c)) }; b(this).each(l); d && "" !== d && g.watchInputs && (clearInterval(b.maskWatchers[d]), b.maskWatchers[d] = setInterval(function () { b(document).find(d).each(l) }, k)); return this }; b.fn.unmask = function () {
        clearInterval(b.maskWatchers[this.selector]);
        delete b.maskWatchers[this.selector]; return this.each(function () { var a = b(this).data("mask"); a && a.remove().removeData("mask") })
    }; b.fn.cleanVal = function () { return this.data("mask").getCleanVal() }; b.applyDataMask = function (a) { a = a || b.jMaskGlobals.maskElements; (a instanceof b ? a : b(a)).filter(b.jMaskGlobals.dataMaskAttr).each(A) }; var p = {
        maskElements: "input,td,span,div", dataMaskAttr: "*[data-mask]", dataMask: !0, watchInterval: 300, watchInputs: !0, watchDataMask: !1, byPassKeys: [9, 16, 17, 18, 36, 37, 38, 39, 40, 91], translation: {
            0: { pattern: /\d/ },
            9: { pattern: /\d/, optional: !0 }, "#": { pattern: /\d/, recursive: !0 }, A: { pattern: /[a-zA-Z0-9]/ }, S: { pattern: /[a-zA-Z]/ }
        }
    }; b.jMaskGlobals = b.jMaskGlobals || {}; p = b.jMaskGlobals = b.extend(!0, {}, p, b.jMaskGlobals); p.dataMask && b.applyDataMask(); setInterval(function () { b.jMaskGlobals.watchDataMask && b.applyDataMask() }, p.watchInterval)
});
*/
﻿(function(m){function z(b,c){function g(a,d){if(a[x]&&a[x]!==p&&a[x]!=="loaded"||d[o])return n;a[S]=a[I]=e;return l}function y(a,d,i){if((i=!!i)||g(a,d)){d[o]=l;for(var j in v)if(v[J](j)&&!v[j][o])return;ia=l;ja()}}function A(a){if(ba(a[w])){a[w]();a[w]=e}}function ua(a,d){if(g(a,d)){d[q]=l;B(function(){k[d[G]].removeChild(a);A(d)},0)}}function va(a,d){if(a[x]===4){a[I]=h;d[q]=l;B(function(){A(d)},0)}}function ca(a,d,i,j,r,C){var K=a[G];B(function(){if("item"in k[K]){if(!k[K][0]){B(arguments.callee,
25);return}k[K]=k[K][0]}var D=s.createElement(L);D.type=i;if(typeof j===t)D.charset=j;if(ba(r)){D[S]=D[I]=function(){r(D,a)};D.src=d}k[K].insertBefore(D,K===u?k[K].firstChild:e);if(typeof C===t){D.text=C;y(D,a,l)}},0)}function ka(a,d,i,j){M[a[T]]=l;ca(a,d,i,j,y)}function la(a,d,i,j){var r=arguments;if(E&&a[q]==e){a[q]=n;ca(a,d,N,j,ua)}else if(!E&&a[q]!=e&&!a[q])a[w]=function(){la.apply(e,r)};else E||ka.apply(e,r)}function ma(a,d,i,j){var r=arguments,C;if(E&&a[q]==e){a[q]=n;C=a.xhr=X?new X("Microsoft.XMLHTTP"):
new m.XMLHttpRequest;C[I]=function(){va(C,a)};C.open("GET",d);C.send("")}else if(!E&&a[q]!=e&&!a[q])a[w]=function(){ma.apply(e,r)};else if(!E){M[a[T]]=l;ca(a,d,i,j,e,a.xhr.responseText);a.xhr=e}}function na(a){if(a.allowDup==e)a.allowDup=c.dupe;var d=a.type,i=a.charset,j=a.allowDup;a=da(a.src,wa);var r=xa(a);if(typeof d!==t)d="text/javascript";if(typeof i!==t)i=e;j=!!j;if(!j&&(M[a]!=e||E&&v[a]||ya(a)))v[a]!=e&&v[a][q]&&!v[a][o]&&r&&y(e,v[a],l);else{if(v[a]==e)v[a]={};j=v[a];if(j[G]==e)j[G]=za;j[o]=
n;j[T]=a;ea=l;if(!aa&&oa&&r)ma(j,a,d,i);else!aa&&pa?la(j,a,d,i):ka(j,a,d,i)}}function qa(a){fa.push(a)}function ga(a){b&&!aa&&qa(a);if(!b||Y)a()}b=!!b;if(c==e)c=O;var ia=n,Y=b&&c[P],pa=Y&&c.cache,aa=Y&&c.order,oa=Y&&c.xhr,Aa=c[Z],za=c.which,wa=c.base,ja=h,ea=n,U,E=l,v={},fa=[],ha=e;Y=pa||oa||aa;U={script:function(){Q(ha);var a=ra(arguments),d=U,i;if(Aa)for(i=-1;++i<a.length;){if(i===0)ga(function(){na(typeof a[0]===t?{src:a[0]}:a[0])});else d=d.script(a[i]);d=d.wait()}else ga(function(){for(i=-1;++i<
a.length;)na(typeof a[i]===t?{src:a[i]}:a[i])});ha=B(function(){E=n},5);return d},wait:function(a){Q(ha);E=n;ba(a)||(a=h);var d=z(l,c),i=d.trigger,j=function(){try{a()}catch(C){}i()};delete d.trigger;var r=function(){if(ea&&!ia)ja=j;else j()};b&&!ea?qa(r):ga(r);return d}};U.block=U.wait;if(b)U.trigger=function(){for(var a,d=-1;a=fa[++d];)a();fa=[]};return U}function F(b){var c,g={},y={UseCachePreload:"cache",UseLocalXHR:"xhr",UsePreloading:P,AlwaysPreserveOrder:Z,AllowDuplicates:"dupe"},A={AppendTo:G,
BasePath:"base"};for(c in y)A[c]=y[c];g.order=!!O.order;for(c in A)if(A[J](c)&&O[A[c]]!=e)g[A[c]]=b[c]!=e?b[c]:O[A[c]];for(c in y)if(y[J](c))g[y[c]]=!!g[y[c]];if(!g[P])g.cache=g.order=g.xhr=n;g.which=g.which===u||g.which===H?g.which:u;return g}var t="string",u="head",H="body",L="script",x="readyState",q="preloaddone",w="loadtrigger",T="srcuri",P="preload",p="complete",o="done",G="which",Z="preserve",I="onreadystatechange",S="onload",J="hasOwnProperty",N="script/cache",e=null,l=true,n=false,s=m.document,
X=m.ActiveXObject,B=m.setTimeout,Q=m.clearTimeout,V=function(b){return s.getElementsByTagName(b)},f=Object.prototype.toString,h=function(){},k={},M={},R=/^[^?#]*\//.exec(m.location.href)[0],$=/^\w+\:\/\/\/?[^\/]+/.exec(R)[0],Ba=V(L),W={},sa=m.opera&&f.call(m.opera)=="[object Opera]",ta=function(b){b[b]=b+"";return b[b]!=b+""}(new String("__count__")),O={cache:!(ta||sa),order:ta||sa,xhr:l,dupe:l,base:"",which:u};O[Z]=n;O[P]=l;k[u]=V(u);k[H]=V(H);var ba=W.isFunc=function(b){return f.call(b)==="[object Function]"},
da=W.canonicalScriptURI=function(b,c){var g=/^\w+\:\/\//;if(typeof b!==t)b="";if(typeof c!==t)c="";b=(g.test(b)?"":c)+b;return(g.test(b)?"":b.charAt(0)==="/"?$:R)+b},ra=W.serializeArgs=function(b){var c=[],g;for(g=-1;++g<b.length;)if(f.call(b[g])==="[object Array]")c=c.concat(ra(b[g]));else c[c.length]=b[g];return c},xa=W.sameDomain=function(b){return da(b).indexOf($)===0},ya=W.scriptTagExists=function(b){for(var c,g=-1;c=Ba[++g];)if(typeof c.src===t&&b===da(c.src)&&c.type!==N)return l;return n};
m.$LAB={setGlobalDefaults:function(b){O=F(b)},setOptions:function(b){return z(n,F(b))},script:function(){return z().script.apply(e,arguments)},wait:function(){return z().wait.apply(e,arguments)},util:W};m.$LAB.block=m.$LAB.wait;(function(b,c,g){if(s[x]==e&&s[b]){s[x]="loading";s[b](c,g=function(){s.removeEventListener(c,g,n);s[x]=p},n)}})("addEventListener","DOMContentLoaded")})(window);
(function(m){function z(p,o,G){function Z(){var f,h,k,M,R,$=[];for(f=0;f<n.length;f++){k=n[f];h=k.test;R=L;if(typeof h===t){M=h.split(".");current=m;for(h=0;h<M.length;h++){current=current[M[h]];if(typeof current==="undefined"){R=H;break}}}else if(T(h))R=!h(k);if(R)$.push(typeof k.alt===t?{src:k.alt,type:k.type,charset:k.charset}:k.alt)}return $}function I(){return N||(G?G():F.setOptions(o))}function S(){try{X()}catch(f){}B()}function J(){q(Q);if(!V){V=H;var f=Z();f.length?F.setOptions(o).script(f).wait(S):
S()}}o=o||{};o[u]=o[u]||P;var N,e,l=[],n=[],s=L,X=w,B=w,Q,V=L;e={script:function(){q(Q);var f=F.util.serializeArgs(arguments),h;for(h=0;h<f.length;h++)if(typeof f[h]!==t&&f[h].alt&&f[h].test){n.push(f[h]);s=H}h=function(){N=I().script.apply(null,f)};p?l.push(h):h();if(s&&!p)Q=x(J,o[u]);return e},wait:function(f){X=T(f)?f:w;var h=S;f=p;var k;if(s){h=J;f=H}f=z(f,o,f?null:function(){return N});B=f.trigger||w;delete f.trigger;k=function(){N=I().wait(h)};p?l.push(k):k();return f}};e.block=e.wait;if(p)e.trigger=
function(){for(var f,h=-1;f=l[++h];)f();l=[];if(s)Q=x(J,o[u]);else{B();triggerNextChange=w}};return e}var F=m.$LAB;if(F){var t="string",u="CDNWaitTime",H=true,L=false,x=m.setTimeout,q=m.clearTimeout,w=function(){},T=F.util.isFunc,P=5E3;m.$LAB={setGlobalDefaults:function(p){if(p[u])P=p[u];F.setGlobalDefaults(p)},setOptions:function(p){return z(L,p)},script:function(){return z().script.apply(null,arguments)},wait:function(){return z().wait.apply(null,arguments)}};m.$LAB.block=m.$LAB.wait}})(window);
LABjs.Net 1.0rc2
Helper library for using ASP.Net with the LABjs library
1/28/2010

Licensing
----------

Copyright (c) 2010 Brant Burnett.  All Rights Reserved.
Written by Brant Burnett <http://www.btburnett.com/> <mailto:btburnett3@gmail.com>
LABjs.Net is distributed under the terms of the GNU Lesser General Public License (GPL)

LABjs.Net is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

LABjs.Net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with LABjs.Net.  If not, see <http://www.gnu.org/licenses/>.

The basic rules of the license are the you can use and change the code here as you see fit,
but you can't remove the license and you can't sell the code.  Please review the license
for more details.

There is a copy of the LABjs library included in this project.  It is copyrighted by
Kyle Simpson, and distributed under the MIT license.

Source Control
---------------

The root branch for LABjs.Net is hosted on GitHub at <http://github.com/btburnett3/LABjs.Net/>.

Overview
---------

LABjs.Net implements an ASP.Net control system for creating LABjs script loading chains.  The core
of this functionality is the LabScriptManager class, which should be included near the top of your
page.  Beneath this, you can list script calls using LabScriptReference or wait calls using
LabWait.

Example:

	<lab:LabScriptManager runat="server">
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jquery/1.4.0/jquery.min.js" />
		<lab:LabWait />
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js" />
	</lab:LabScriptManager>
	
The same options that are available in $LAB.setOptions() are also available as properties on the LabScriptManager.
For more information, please visit the LABjs documentation at <http://labjs.com>.  You may also use the LabUrl
property to specify an alternate URL to download LABjs from, rather than using the copy embedded in the
assembly.

Script References
------------------

LabScriptReferences can use either the Path property or the Assembly and Name properties, just like the ASP.Net
AJAX ScriptReferences.  Application relative paths are allowed.

LabScriptReferences also have a ScriptMode option, which works just like the ScriptMode option on a ScriptReference
to specify if the release or debug version of the script should be used.  However, LABjs.Net also adds a
DebugNameStyle property as well.  By default, it uses the setting from the LabScriptManager, which defaults to
AddDebug, but you can override it for each script.  AddDebug substitutes .debug.js for the .js at the end of the
filename, which is the ASP.Net standard.  Using RemoveMin will instead change the .min.js on the end of the
filename to a plain .js.  This is more consistent with the naming standards used by a lot of javascript libraries.

There are also more options, which are passed on to the script() call on the $LAB chain.  For more information
about these options, please visit the LABjs documentation at <http://labjs.com>.

Combined Scripts
-----------------

So long as your site is running ASP.Net AJAX, you can also take advantage of script combining.  Note that you aren't
necessarily required to include a ScriptManager on your page, but you need the modules and handlers in place.  To
combine a set of scripts, just wrap the LabScriptReference objects in a LabScriptCombine.  The script files
will be combined into a single file for download via a single HTTP request, in the order they are specified.  No
matter how many scripts are included in the LabScriptCombine, a single script() call is added to chain.

LabScriptCombine supports the same set of LABjs options to be included in the chain as LabScriptReference does.  Note
that any LABjs options that you set on the LabScriptRefernce objects inside the combine will be ignored.  Only the
options on the LabScriptCombine are used.

Note that for the sake of efficiency you should combine the same scripts together at different places on your site,
and in the same order.  Each combination that you use is a different copy that is downloaded and cached by the client.
Also, you can only include scripts that are either assembly resources or application relative (using ~/).  Any other
kind of URL will throw an exception.

Example:

	<lab:LabScriptManager runat="server">
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jquery/1.4.0/jquery.min.js" />
		<lab:LabWait />
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js" />
		<lab:LabWait />
		<lab:LabScriptCombine>
			<lab:labScriptReference Path="~/js/script.js" />
			<lab:labScriptRefernece Path="~/js/script2.js" />
		</lab:LabScriptCombine>
	</lab:LabScriptManager>

Wait Functions
---------------

If you want to include functions that are called as part of a wait call, just include it inline in the LabWait
tag.  You can optionally wrap it in a <script type="text/javascript"></script> tag so that syntax highlighting
and code formatting in Visual Studio will still work correctly.

Example:

	<lab:LabScriptManager runat="server">
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jquery/1.4.0/jquery.min.js" />
		<lab:LabWait>
			<script type="text/javascript">
				$('body').append('<p>Testing!</p>');
			</script>
		</lab:LabWait>
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js" />
	</lab:LabScriptManager>

CDN Failovers (Experimental)
-----------------------------

This library also includes an experimental copy of the cdnLAB.js library that I've been working on.  This implements
automatic failover to a local file if a CDN doesn't return a valid script.  Note that for some browsers, this results
in a delay because the only way we know the script failed to load is to check the status after a timeout.

To enable this functionality, add EnableCdnFailover="true" to the LabScriptManager, and add Alternate and Test properties
to the LabScriptReference tags.  You may also optionally specify CdnWaitTime="x" in milliseconds on the LabScriptManager,
the default is 5 seconds.

Alternate on the LabScriptReference should specify the URL of the alternate script file to load, application relative paths
are allowed.

Test on the LabScriptReference specifies how to test to be certain that the library loaded successfully.  This can be
one of three things:

	1.  A property name on the window object.  If this property != undefined, the library is assumed to have loaded.
	(i.e. "jQuery")
	
	2.  A "." delimited list property chain, starting from the window object.  If all the properties along the chain
	are != undefined, the library is assumed to have loaded.  (i.e. "jQuery.ui")
	
	3.  A function which should return true if the library loaded successfully.  The javascript object passed to the
	script() call will be passed to this function as a parameter.
	
Example:
	
	<lab:LabScriptManager runat="server" EnableCdnFailover="true" CdnWaitTime="10000">
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jquery/1.4.0/jquery.min.js"
			Alternate="~/js/jquery-1.4.0.min.js" Test="jQuery" />
		<lab:LabWait>
			<script type="text/javascript">
				$('body').append('<p>Testing!</p>');
			</script>
		</lab:LabWait>
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js"
			Alternate="~/js/jquery-ui-1.7.2.min.js" Test="jQuery.ui" />
	</lab:LabScriptManager>
	
Proxies
--------

Much like ScriptManagerProxy, there is the LabScriptManagerProxy class.  This allows you to add script and wait
calls to the $LAB chain from within content pages and user controls.  Any LabScriptReference or LabWait objects
included in the proxy will be appended to the chain of the main LabScriptManager class.

You may control the order in which multiple ScriptManagerProxy controls are added to the chain by setting the
Priority property.  Proxies with the same priority will be added in page order.  Be default, all proxies have
a priority of 0.

Master Page Example:

	<lab:LabScriptManager runat="server">
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jquery/1.4.0/jquery.min.js" />
		<lab:LabWait>
			<script type="text/javascript">
				$('body').append('<p>Testing!</p>');
			</script>
		</lab:LabWait>
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js" />
	</lab:LabScriptManager>

Content Page Example:

	<lab:LabScriptManagerProxy runat="server">
		<lab:LabScriptReference Path="~/js/jquery.metadata.min.js" />
	</lab:LabScriptManagerProxy>

Groups and Named Waits
-----------------------

When adding scripts using a Proxy, you might want to add scripts earlier in the chain, rather than appending all
of them to the end.  This can be done by implementing two changes.  First, add a Name property to a LabWait
in the main LabScriptManager.  This named LabWait acts as the insertion point.  Then, add a collection of scripts
using the LabActionGroup object rather than just plain LabScriptReference objects.  The LabActionGroup has an
InsertAt property which should reference the name of the LabWait.

LabActionGroup objects can contain as many LabScriptReferences as you would like.  You may also include a single
LabWait at the very end of the chain.  If you do, the inline function in this LabWait will be merged with the named
LabWait being referenced.  The merged code will be in order based on the order the proxies are processed.

You may include multiple LabActionGroup objects in a single proxy, as well as add other script references to the
end of the chain in the same proxy.

Master Page Example:

	<lab:LabScriptManager runat="server">
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jquery/1.4.0/jquery.min.js" />
		<lab:LabWait Name="jQuery">
			<script type="text/javascript">
				$('body').append('<p>Testing!</p>');
			</script>
		</lab:LabWait>
		<lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js" />
	</lab:LabScriptManager>

Content Page Example:

	<lab:LabScriptManagerProxy runat="server">
		<lab:LabActionGroup InsertAt="jQuery">
			<lab:LabScriptReference Path="~/js/jquery.metadata.min.js" />
			<lab:LabWait>
				<script type="text/javascript">
					$('body').append('<p>Testing 2!</p>');
				</script>
			</lab:LabWait>
		</lab:LabActionGroup>
	</lab:LabScriptManagerProxy>
	
Output Chain Example:

	$LAB
		.script("http://ajax.googleapis.com/ajax/libs/jquery/1.4.0/jquery.min.js")
		.script("/js/jquery.metadata.min.js")
		.wait(function() {
			$('body').append('<p>Testing!</p>');
			$('body').append('<p>Testing 2!</p>');
		})
		.script("http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js");
		
Change Log
-----------

1.0rc2
	Updated cdnLABjs to the latest version
	Added LabScriptCombine class, which supports ASP.Net AJAX script combining via a reflection hack
		(Note: As usual, leave it to Microsoft to take perfectly useful functionality and put it where you can't get to it)

1.0rc1
	Initial testing release
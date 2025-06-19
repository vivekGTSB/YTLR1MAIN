<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.DocumentsDate" Codebehind="DocumentsDate.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Documents Date Management</title>
   <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/jquery-ui.css";
        @import "cssfiles/jquery.ui.dialog.css";
        @import "cssfiles/TableTools.css";
        @import "cssfiles/ColVis.css";
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
     <script src="jsfiles/jquery-ui.min.js" type="text/javascript"></script>
    <script type="text/javascript" >
        $(function () {
            $("#tabs").tabs();
//            $("#tabs").tabs().addClass("ui-tabs-vertical ui-helper-clearfix");
//            $("#tabs li").removeClass("ui-corner-top").addClass("ui-corner-left");
        });

        function init() {
            document.getElementById("mainframe1").height = $(window).height() - 32 + "px";
            document.getElementById("mainframe2").height = $(window).height() - 32 + "px";
            document.getElementById("mainframe1").width = "1000px";
            document.getElementById("mainframe2").width = "1000px";

            document.getElementById("mainframe3").height = $(window).height() - 32 + "px";
            document.getElementById("mainframe3").width = "1000px";
        }
        function resize() {
            document.getElementById("mainframe1").height = $(window).height() - 32 + "px";
            document.getElementById("mainframe2").height = $(window).height() - 32 + "px";
            document.getElementById("mainframe3").height = $(window).height() - 32 + "px";
        }
        function changetitle(type) {
            if (type == 1)
                window.parent.frames[1].document.title = "Documents Date Management";
            else if (type == 2)
                window.parent.frames[1].document.title = "Trailer Management";
            else if (type == 3)
                window.parent.frames[1].document.title = "Driver Alerts Management";
        }
  </script>
</head>
<body onload="init()" onresize="resize()">
    <form id="form1" runat="server">
    <br />
    <center>
     
  <div style="width:100%;">
  <div id="tabs">
        <ul>
            <li><a href="#tabs-1" onclick="changetitle(1)">Vehicle</a></li>
            <li><a href="#tabs-2" onclick="changetitle(2)">Trailer</a></li>
              <li><a href="#tabs-3" onclick="changetitle(3)">Driver</a></li>
        </ul>
        <div id="tabs-1">
         <iframe id="mainframe1" name="mainframe1" frameborder="0" scrolling="Auto" marginheight="0"
                    marginwidth="0" width="100%" height="100%" src="DocumentsDateManagement.aspx" style="width: 100%;">
                </iframe>
          
        </div>
        <div id="tabs-2">
            <iframe id="mainframe2" name="mainframe2" frameborder="0" scrolling="Auto" marginheight="0"
                    marginwidth="0" width="100%" height="100%" src="TrailerManagement.aspx"  style="width: 100%;">
                </iframe>
        </div>
          <div id="tabs-3">
            <iframe id="mainframe3" name="mainframe3" frameborder="0" scrolling="Auto" marginheight="0"
                    marginwidth="0" width="100%" height="100%" src="DriverAlertsManagement.aspx"  style="width: 100%;">
                </iframe>
        </div>
    </div>
  </div>  
   
    </center> 
    
    </form>
</body>
</html>

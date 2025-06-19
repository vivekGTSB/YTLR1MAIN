<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.GeofenceManagementLafarge" Codebehind="GeofenceManagementLafarge.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Geofence Management</title>
     <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
         </style>

    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    
    <style type="text/css" >
      
       
       
       .dataTables_filter 
        {
            width:49%;
        }
       table.display thead th div.DataTables_sort_wrapper {
position: relative;
padding-right: 0px;
}
     table.display thead th {
padding: 0px 0px 0px 0px;
cursor: pointer;
}
     
 table.display tfoot th {
padding: 0px 0px 0px 0px;
font-weight: bold;
}

      tfoot input {
margin: 0.2em 0;
width: 100%;
color: #444;
} 
   
     
       a
     {
         text-decoration:none;
     }
    </style>
   

      <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript">

        $(function () {

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: 1000,
                minHeight: 529
            });
        });


        function openpage(lat, lon, id) {
            var ll = lat + "," + lon;
            openMap("Gmap.aspx?qs=" + ll + "&id=" + id);
            // window.open("Gmap.aspx?qs=" + ll + "&id=" + id, '_blank', 'fullscreen=yes,titlebar=no,scrollbars=yes,width=900px,height=500px');
        }
        function openMap(message) {
            document.getElementById("gmappage").src = message;
            $("#dialog-message").dialog("open");
        }
        function deleteconfirmation() {
            var checked = false;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    if (elm.checked == true) {
                        checked = true;
                        break;
                    }
                }
            }
            if (checked) {
                var result = confirm("Are you sure you want to delete ?");
                if (result) {
                    return true;
                }
                return false;
            }
            else {
                alert("Please select checkboxes");
                return false;
            }
        }
        function activeconfirmation() {
            var checked = false;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    if (elm.checked == true) {
                        checked = true;
                        break;
                    }
                }
            }
            if (checked) {
                return true;
            }
            else {
                alert("Please select checkboxes to activate/in-activate geofences");
                return false;
            }
        }
        function checkall(chkobj) {
            var chkvalue = chkobj.checked;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    document.forms[0].elements[i].checked = chkvalue;
                }
            }
        }
        function mouseover(x, y) {
            document.getElementById("mapimage").src = "images/maploading.gif";
            document.getElementById("mapimage").src = "GussmannMap.aspx?x=" + x + "&y=" + y;
        }
        function displayalert() {
            alert("Sorry.. You cann't add geofences from here.\n You can only add geofences from Map.");
        }
        function ExcelReport() {

            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();

        }
        function ShowGoogleMaps() {
            var googlemapsformobj = document.getElementById("googlemapsform");
            googlemapsformobj.action = "<%=googlemapsparameters%>" + "" + "?r=" + Math.random();
            googlemapsformobj.submit();

        }
        function ShowGoogleEarth() {
            var googleearthformobj = document.getElementById("googleearthform");
            googleearthformobj.action = "<%=googleearthparameters%>"
            googleearthformobj.submit();
        }

    </script>
    <script type="text/javascript">

        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-32500429-1']);
        _gaq.push(['_setDomainName', 'avls.com.my']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'https://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();
        function hidebtns() {
            if (document.getElementById("tf").value == "0") {
                document.getElementById("ImageButton1").style.visibility = "hidden";
                document.getElementById("ImageButton2").style.visibility = "hidden";
                document.getElementById("ImageButton3").style.visibility = "hidden";
                document.getElementById("ImageButton4").style.visibility = "hidden";
            }
        }
        function fnFeaturesInit() {

            $('ul.limit_length>li').each(function (i) {
                if (i > 10) {
                    this.style.display = 'none';
                }
            });

            $('ul.limit_length').append('<li class="css_link">Show more<\/li>');
            $('ul.limit_length li.css_link').click(function () {
                $('ul.limit_length li').each(function (i) {
                    if (i > 5) {
                        this.style.display = 'list-item';
                    }
                });
                $('ul.limit_length li.css_link').css('display', 'none');
            });
        }
        $(document).ready(function () {
            fnFeaturesInit();
            $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 100,
                "aaSorting": [[2, "asc"]],
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(1)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                "aoColumnDefs": [
                          { "bVisible": true, "bSortable": false, "aTargets": [0, 1] }
                              ]
            });
        });


    </script>
   
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="fuelform" runat="server">
    <center>
        <br />
      
         <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Private Geofence Management [YTL]</b>
        <br />
        <br />
 
     
  
     <table style="font-family: Verdana; font-size: 11px; width:762px"  >

        <tr>
                <td align="left">
                <% If Request.Cookies("userinfo")("customrole") = "LFSuperUser" Then%>
                    <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Active" ToolTip="Active" />
                    <asp:Button ID="ImageButton2" class="action blue" runat="server" Text="Inactive" ToolTip="Inactive" />
                      <asp:Button ID="ImageButton5" class="action blue" runat="server" Text="Delete"  ToolTip="Delete" OnClientClick="return deleteconfirmation();" />
                      <%End If%>
                </td>
                <td align="center">
                 
                </td>
               <td align="right" valign="middle" >
                    <img src="images/GoogleMaps.gif" alt="Show Vehicles in Google Maps" style=" cursor: pointer; vertical-align:middle; visibility:hidden ; "
                        onclick="ShowGoogleMaps();"  />
                    <img src="images/GoogleEarth.gif" alt="Show Vehicles in Google Earth" style="cursor: pointer; vertical-align:middle ; visibility:hidden ; "
                        onclick="ShowGoogleEarth();" />
                    <a href="javascript:ExcelReport();" class="button" >Save Excel </a>
                       
                </td>
            </tr>

    <tr>
    <td colspan="3" align="left">
        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px;
            font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif"; width="760px">
            <%=sb1.ToString()%>
          </table> 
        </td>
         </tr>
         <tr>
         
           <td ><% If Request.Cookies("userinfo")("customrole") <> "LFSuperUser" Then%>
                    <asp:Button ID="ImageButton3" class="action blue" runat="server" Text="Active" ToolTip="Active" />
                    <asp:Button ID="ImageButton4" class="action blue" runat="server" Text="Inactive"  ToolTip="Inactive" />
                    <asp:Button ID="ImageButton6" class="action blue" runat="server" Text="Delete" OnClientClick="return deleteconfirmation();" ToolTip="Delete" />
                    <%End if %>
                </td>
                <td colspan="2" align="right">
                </td>
            </tr>
        
        </table> 
            </center> 
          
  
  
    <input type="hidden" name="tf" id="tf" value="<%= tf %>" />
    </form>
    <form id="googlemapsform" method="post" action="" target="_blank">
    </form>
    <form id="googleearthform" method="post" action="">
    </form>
    <form id="excelform" method="get" action="PubGeoExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="YTL Private Geofence" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    <input type="hidden" id="excelquery" name="excelquery" value="" />
  
    </form>
    
</body>

<div id="dialog-message" title="G Map" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; padding-left: 0px;">
    <iframe id="gmappage" name="gmappage" src="" frameborder="0" scrolling="auto" height="512" width="998px" style="border: solid 1px #aac6ff;" />
</div>
</html>

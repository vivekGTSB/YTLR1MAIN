<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.VehiclesListNew" Codebehind="VehiclesListNew.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vehicles List</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/style.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/demos22.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/jquery.ui.dialog.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.Min.js"></script>
    <script type="text/javascript">
    
    var ec=<%=ec %>;
    function ExcelReport()
{
       var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
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
    function refreshTable() {
        document.getElementById("ss").value = $("#ddluser1").val();
        document.getElementById("form1").submit();
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
    var oTable;
    function UpdateData() {
        var doc = window.parent.frames[0].frames[0].document;
         var newodometer=doc.getElementById("txtodometer").value;
          var oldodometer=$("#hdOdo").val();
          var newrecdate =doc.getElementById("txtRecDate").value;
          var oldrecdate =$("#hdrecdate").val();
           if (newodometer!=oldodometer || newrecdate!=oldrecdate)
            {
              confirmbox("Are you Confirm the Odometer Reading "+newodometer+" on Date "+ newrecdate +" "+doc.getElementById("ddlbh").value +":"+doc.getElementById("ddlbm").value + " ?");
                        $("#dialog-confirm").dialog({
                            close: function (event, ui) {
                                if (confirmresult) {
                                    var str = "UpdateVehicleInfo.aspx?plateno=" + doc.getElementById("lblPlateno").innerHTML + "&groupname=" + doc.getElementById("ddlgroupname").value + "&type=" + doc.getElementById("txtType").value + "&brand=" + doc.getElementById("txtBrand").value + "&model=" + doc.getElementById("txtModel").value + "&speedlimit=" + doc.getElementById("txtSpeed").value + "&drivermobile=" + doc.getElementById("txtphone").value + "&odometer=" + doc.getElementById("txtodometer").value + "&baseplant=" + doc.getElementById("ddlbaseplant").value + "&pmid=" + doc.getElementById("txtpmid").value + "&recdate=" + doc.getElementById("txtRecDate").value + " " + doc.getElementById("ddlbh").value + ":" + doc.getElementById("ddlbm").value + "&r=" + Math.random() + "&permit=" + doc.getElementById("ddlpermit").value;
                                $.get(str, function (data) {
                                $("#dialog-message").dialog("close");
                                document.forms[0].submit();
                                   });       
                               } 
                            }

                        });
            }
            else
            {
               var str = "UpdateVehicleInfo.aspx?plateno=" + doc.getElementById("lblPlateno").innerHTML + "&groupname=" + doc.getElementById("ddlgroupname").value + "&type=" + doc.getElementById("txtType").value + "&brand=" + doc.getElementById("txtBrand").value + "&model=" + doc.getElementById("txtModel").value + "&speedlimit=" + doc.getElementById("txtSpeed").value + "&drivermobile=" + doc.getElementById("txtphone").value + "&odometer=" + doc.getElementById("txtodometer").value + "&baseplant=" + doc.getElementById("ddlbaseplant").value + "&pmid=" + doc.getElementById("txtpmid").value + "&recdate=" + doc.getElementById("txtRecDate").value + " " + doc.getElementById("ddlbh").value + ":" + doc.getElementById("ddlbm").value + "&r=" + Math.random() + "&permit=" + doc.getElementById("ddlpermit").value;
                                $.get(str, function (data) {
                                $("#dialog-message").dialog("close");
                                document.forms[0].submit();
                                   });     
            }
    }
    $(document).ready(function () {
        $("#dialog:ui-dialog").dialog("destroy");

        $("#dialog-message").dialog({
            resizable: false,
            draggable: false,
            modal: true,
            autoOpen: false,
            width: 350,
            minHeight: 360,
            height: 360,
            buttons: {
                Update: function () {
                    UpdateData();
                },
                Close: function () {
                    $(this).dialog("close");
                }
            }
        });

          $("#dialog:ui-dialog").dialog("destroy");
             $("#dialog-msg").dialog({
                autoOpen: false,
                resizable: false,
                height: 140,
                modal: true,
                buttons: {
                    "Ok": function () {
                        $(this).dialog("close");
                    }

                }
            });

            $("#dialog:ui-dialog").dialog("destroy");
            $("#dialog-confirm").dialog({
                resizable: false,
                draggable: false,
                height: 160,
                modal: true,
                autoOpen: false,
                buttons: {
                    "Yes": function () {
                        confirmresult = true;
                        $(this).dialog("close");
                    },
                    "No": function () {
                        confirmresult = false;
                        $(this).dialog("close");

                    }
                }
            });

        var isSvwong = <%=isSvwong %>;
//  if (isSvwong == true ) {
//    $("#exceldiv").show();
//}
//else {
//     $("#exceldiv").hide();
//}
        fnFeaturesInit();
        oTable = $('#examples').dataTable({
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "iDisplayLength": 100,
            "aaSorting": [[1, "asc"]],
            "aLengthMenu": [10, 25, 50, 100, 200, 500],
            "bLengthChange": false,
            "fnDrawCallback": function (oSettings) {
                if (oSettings.bSorted || oSettings.bFiltered) {
                    if (oSettings.aoColumns[0].bVisible == true) {
                        for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                            $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                        }
                    }
                }
            },
            "sDom": '<"H"Cl<"MyButton">f>rt<"F"iTp>',
            "aoColumnDefs": [
                          { "bVisible": true, "bSortable": false, "aTargets": [0] }
                             ]
        });

        $("div.MyButton").html('<div> <%=opt %> </div>');
        jQuery(".chosen").data("placeholder", "Select ...").chosen();

    });
          function openPopup(userid, plateno, grpid, brand, model, speedlimit,type,mob,odometer,recdate,pmid,baseplant,itype,permit) {
             // var x = userid + "," + plateno + "," + groupname + "," + brand + "," + model + "," + speedlimit;
             // alert(x);
                 $("#hdrecdate").val(recdate);
                 $("#hdOdo").val(odometer);
              document.getElementById("idlingpage").style.visibility = "visible";
              document.getElementById("idlingpage").src = "VehiclePopup.aspx?userid=" + userid + "&plateno=" + plateno + "&grpid=" + grpid + "&brand=" + brand + "&model=" + model + "&speedlimit=" + speedlimit + "&type=" + type + "&mob=" + mob + "&odometer=" + odometer + "&recdate=" + recdate + "&pmid=" + pmid + "&baseplant=" + baseplant + "&internal=" + itype + "&permit=" + permit;
              $("#dialog-message").dialog("open");

          }
          function idlingpage_onclick() {

          }

          function ShowGoogleEarth()
{
    var googleearthformobj=document.getElementById("googleearthform");
    googleearthformobj.action="ShowVehiclesInGoogle.aspx?<%=googleearthparameters%>"
    googleearthformobj.submit();     
}
 function confirmbox(confirmMessage) {
            confirmresult = false;
            document.getElementById("displayc").innerHTML = confirmMessage;
            $("#dialog-confirm").dialog("open");
        }
         function alertbox(message) {
            if (message == "") {
                document.getElementById("displayp").innerHTML = message;

            }
            else {
                document.getElementById("displayp").innerHTML = message;
                $("#dialog-msg").dialog("open");
            }
        }

    </script>
    <link href="cssfiles/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
    <style type="text/css">
        .dataTables_filter
        {
            width: 50%;
            float: right;
            text-align: right;
            margin-top: -24px;
        }
        .chzn-container .chzn-results li
        {
            line-height: 80%;
            padding-bottom: 8px;
            padding-top: 8px;
            margin: 0;
            list-style: none;
            z-index: 1;
            color: #4E6CA3;
        }
        .chzn-search
        {
            width: 203px;
        }
        table.display td
        {
            padding: 2px 2px;
        }
    </style>
</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="form1" runat="server">
    <center>
        <br />
        <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Vehicle List</b>
        <br />
        <br />
        <table border="0" width="940px;" style="font-family: Verdana; font-size: 11px">
            <tr>
                <td align="left">                  
                    <asp:RadioButtonList runat="server" AutoPostBack="true"  RepeatDirection="Horizontal" ID="radioTransporterType" OnSelectedIndexChanged="radioTransporterType_SelectedIndexChanged">
                        <asp:ListItem Text="All"  Value="2" Selected="True" />
                        <asp:ListItem Text="Internal Transporter"  Value="1"  />
                        <asp:ListItem Text="External Transporter" Value="0" />
                          <asp:ListItem Text="Transporter With Out GPS" Value="3" />
                    </asp:RadioButtonList>
                </td>
                <td align="right">
                    <a id="exceldiv" href="javascript:ExcelReport();" class="button"><span class="ui-button-text"
                        title="Download Excel">Excel</a>
                </td>
            </tr>
            <tr>
                <td align="left" colspan="2">
                    <div id="fw_container">
                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px;
                            font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
                            <%=sb1.ToString()%>
                        </table>
                        
                    </div>
                </td>
            </tr>
        </table>
    </center>
    <input type="hidden" value="" id="ss" runat="server" />
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="plateno" name="plateno" value="" />
    <input type="hidden" id="title" name="title" value="Vehicles List" />
    <input type="hidden" name="odo" value="" runat="server" id="hdOdo" />
    <input type="hidden" name="recdate" value="" runat="server" id="hdrecdate" />
    </form>
    <form id="googleearthform" method="post" action="">
    </form>
    <div id="dialog-confirm" title="Conform Dialog">
        <p id="displayc">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-msg" title="Alert">
        <p id="displayp">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-message" title="Vehicle's Information" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="idlingpage" name="idlingpage" src="" frameborder="0" scrolling="no" height="283px"
            width="310px" style="visibility: hidden;"  />
    </div>
</body>
</html>

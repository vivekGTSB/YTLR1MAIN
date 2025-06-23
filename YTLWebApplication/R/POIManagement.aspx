<%@ Page Language="VB" AutoEventWireup="false" Debug="true" EnableEventValidation="false" Inherits="YTLWebApplication.POIManagement" Codebehind="POIManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Private POI Management</title>
     <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
      
   <style type="text/css">
      .dataTables_filter {
width: 50%;
float: right;
text-align: right;
margin-top: -24px;
}
         .chzn-container .chzn-results li {
line-height: 80%;
padding-bottom: 8px;
padding-top: 8px;
margin: 0;
list-style: none;
z-index: 1;
color: #4E6CA3;
}


div.dataTables_wrapper .ui-widget-header {
font-weight: normal;
float: left;
text-align: left;
}
   .dataTables_wrapper .ui-toolbar {

padding: 5px;
width: 838px;
}
table.display tfoot th {
padding: 0px 0px 0px 0px;
font-weight: bold;
}
table.display thead th {
padding: 0px 0px 0px 0px;
font-weight: bold;
}
   table.display thead th div.DataTables_sort_wrapper {
position: relative;
padding-right: 0px;
padding-left: 0px;
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
        function checkall(chkobj) {
            var chkvalue = chkobj.checked;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    document.forms[0].elements[i].checked = chkvalue;
                }
            }
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
                var result = confirm("Are you delete checked Private POIs ?");
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
        function refreshTable() {
            document.getElementById("ss").value = $("#ddluser1").val();
                 document.form1.submit();
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
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 100,
                "aaSorting": [[2, "asc"]],
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "bLengthChange": false,
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(1)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                   "sDom": '<"H"Cl<"MyButton">f>rt<"F"iTp>',
                   "aoColumnDefs": [
                          { "bVisible": true, "bSortable": false, "aTargets": [0,1] }
                             ]
               });

            $("div.MyButton").html('<div>Username : <%=opt %> </div>');
              jQuery(".chosen").data("placeholder", "Select Frameworks...").chosen();
  
        });
            
    function ExcelReport()
    {
        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    }
    </script>
      <link href="cssfiles/chosen.css" rel="stylesheet" type="text/css" />
     <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
</head>
<body style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="form1" runat="server">

    <center>
        <br />
         <b style="font-family: Verdana; font-size: 20px; color: #38678B;">POI Management</b>
        <br />
       <table style="font-family: Verdana; font-size: 11px; width:700px">
            <tr>
                <td align="left">
                    <asp:Button ID="ImageButton1"  class="action blue"
                        runat="server" Text="Delete" ToolTip="Delete" />
                      </td>
                <td align="center">
                 
                </td>
                <td align="right" valign="middle">
               <a href="javascript:ExcelReport();" class="button"><span class="ui-button-text" title="Save to Excel"></span>
                                            Save&nbsp;Excel</a>
                    <a href="<%=addPOI %>" class="button" title="Add New POI" style="width: 59px;">
                        <span class="ui-button-text">Add </span></a>

                </td>
            </tr>
          <tr>
                <td align="center" colspan="3">
                 <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px;
            font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif"; width="850px">
            <%=sb1.ToString()%>
          </table> 
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:Button ID="ImageButton2" class="action blue"
                        runat="server" Text="Delete" ToolTip="Delete" />
                </td>
                <td align="center" >
                </td>
                <td align="right" valign="middle">
                    <a href="<%=addPOI %>" class="button" title="Add New POI" style="width: 59px;">
                        <span class="ui-button-text">Add </span></a>
                </td>
            </tr>
        </table> 
   <input type="hidden" value="" id="ss" runat="server" />
       </form>
       <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="POI Management" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>

<%@ Page Language="VB" AutoEventWireup="false" Debug="false" EnableEventValidation="true" Inherits="YTLWebApplication.CapitalManagement" Codebehind="CapitalManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Capital Management</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="X-Content-Type-Options" content="nosniff">
    <meta http-equiv="X-Frame-Options" content="DENY">
    <meta http-equiv="X-XSS-Protection" content="1; mode=block">
    
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
        a {
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
      
        function refreshTable() {
            var selectedUser = $("#ddluser1").val();
            if (selectedUser && selectedUser !== "0") {
                document.getElementById("ss").value = selectedUser;
                document.form1.submit();
            }
        }
        
        function ExcelReport() {
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();
        }
    </script>
    
    <script type="text/javascript">
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
                    { "bVisible": true, "bSortable": false, "aTargets": [0] },
                    { "bVisible": true, "bSortable": true, "aTargets": [1] }
                ]
            });

            $("div.MyButton").html('<div>Username : <%=SafeJavaScript(opt) %> </div>');
            jQuery(".chosen").data("placeholder", "Select Frameworks...").chosen();
        });
    </script>
    
    <link href="cssfiles/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
</head>
<body style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="form1" runat="server">
        <%=GetCSRFTokenHtml()%>
        <center>
            <br />
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Capital Management</b>
            <br />
            <br />
            <table style="font-family: Verdana; font-size: 11px; width:700px">
                <tr>
                    <td align="center" colspan="3">
                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 11px; font-weight: normal; font-family: Verdana; width: 100%;">
                            <%=SafeOutput(sb1.ToString())%>
                        </table> 
                    </td>
                </tr>
            </table> 
            <input type="hidden" value="" id="ss" runat="server" />
        </center>
    </form>
    
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="Capital Management" />
        <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>
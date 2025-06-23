<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.LafargeDriverSubmissionReport" Codebehind="LafargeDriverSubmissionReport.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>e-POD Management</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        .dataTables_info
        {
            width: 25%;
            float: left;
        }
    </style>




    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">
          
		
		    $(function () {
          $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: new Date(2017, 10, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
          });

          $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: new Date(2013, 10, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

          });
          
            $("#dialog:ui-dialog").dialog("destroy");
            
            $("#MyDiv").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: 1000,
                minHeight: 529,
                buttons: {
                    //Accept: function () {
                    //    UpdateData(2);
                    //    $(this).dialog("close");
                    //},
                    //Reject: function () {
                    //    UpdateData(3);
                    //    $(this).dialog("close");
                    //},
                    Close: function () {
                        $(this).dialog("close");
                    }
                }

            });           
      });

        function openMe(patchno) {
            document.getElementById("hdnP").value = patchno;
            document.getElementById("driverData").src= "ViewUploadedData.aspx?p=" + patchno + "&r=" + Math.random();
            document.getElementById("driverData").style.visibility = "visible";
            $("#MyDiv").dialog("open");
        }
        function UpdateData(status_code) {
            var patchno = document.getElementById("hdnP").value;
            var lastXHR = $.get("UpdateJobStatus.aspx?i=" + status_code + "&p=" + patchno, function (data) {
                if (data == "Yes") {
                    alert("Successfully updated the status.");
                    return false;
                }
                else {
                    alert("Sorry.Please try again..!!!");                    
                    return false;
                }
            });

        }
    
     
function mysubmit()
{
    var username=document.getElementById("DropDownList1").value;
    if (username=="--Select User Name--")
    {
         alert("Please select user name");
         return false;         
    }
    else if (document.getElementById("ddlplate").value=="--Select Plate No--")
    {
         alert("Please select plate no");
         return false;         
    }
     
    
   
    var bigindatetime=document.getElementById("txtBeginDate").value+" "+document.getElementById("ddlbh").value+":"+document.getElementById("ddlbm").value;
    var enddatetime=document.getElementById("txtEndDate").value+" "+document.getElementById("ddleh").value+":"+document.getElementById("ddlem").value;
    
    var fdate=Date.parse(bigindatetime);
    var sdate=Date.parse(enddatetime);
    
    var diff=(sdate-fdate)*(1/(1000*60*60*24));
    var days=parseInt(diff)+1;
    if(days>5)
    {
        return confirm("You selected "+days+" days of data.So it will take more time to execute.\nAre you sure you want to proceed ? ");
    }
    return true;
     
}
var ec=<%=ec %>;
function ExcelReport()
{
    if(ec==true)
    {
        var plateno=document.getElementById("ddlplate").value;
       
        document.getElementById("plateno").value=plateno;

        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    }
    else
    {
        alert("First click submit button");
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
		       fnFeaturesInit()
		        $('#examples').dataTable({
		            "bJQueryUI": true,
		            "sPaginationType": "full_numbers",
                    "iDisplayLength": 25,
                   "aLengthMenu": [ 10,25,50,100,200,500 ],
                    "fnDrawCallback": function (oSettings) {
                       if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                              //  $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                   "aoColumnDefs": [
                          {   "bVisible": true, "bSortable": false, "aTargets": [0] } ,
                           
                           
                             { "aDataSort": [ 1,5 ], "aTargets": [1] },
                              { "aDataSort": [ 4,1,5 ], "aTargets": [4] }
                              ] } );
		          });
            function ExcelReport() {
                var excelformobj = document.getElementById("excelform");
                excelformobj.submit();
                return false;
            }

           
    </script>
    
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="Form1" method="post" runat="server">
    <center>
        <div>
            <br />
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">e-POD Management</b>
            <br />
            <br />
        </div>
        <table>
            <tr>
                <td align="center">
                    <table style="font-family: Verdana; font-size: 11px;">
                        <tr>
                            <td colspan="2" style="height: 20px; background-color: #465ae8;" align="left">
                                <b style="color: White;">&nbsp;e-POD Management&nbsp;:</b>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 450px; border: solid 1px #5B7C97;">
                                <table style="width: 450px;">
                                    <tbody>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">Begin Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                    id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">End Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                    id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" />
                                            </td>
                                        </tr>
                                      <tr>
                                    <td align="left">
                                        <b style="color: #5f7afc;">Source</b>
                                    </td>
                                    <td>
                                        <b style="color: #5f7afc;">:</b>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList ID="ddlSource" runat="server" Font-Size="12px" Width="260px" Font-Names="verdana"
                                            EnableViewState="False">
                                            <asp:ListItem Value="ALL SOURCES" Selected="True"> ALL SOURCES </asp:ListItem>
                                            <asp:ListItem Value="'YL20','VL20','WL20','XL20'">Langkawi (LW)</asp:ListItem>
                                            <asp:ListItem Value="'YL21','VL21','WL21','XL21'">Pasir Gudang Terminal (PGT)</asp:ListItem>
                                            <asp:ListItem Value="'YL10','VL10','WL10','XL10'">Rawang (RW)</asp:ListItem>
                                            <asp:ListItem Value="'YL11','VL11','WL11','XL11'">Kanthan (KW)</asp:ListItem>
                                            <asp:ListItem Value="'YL12','VL12','WL12','XL12'">PASIR GUDANG 1</asp:ListItem>
                                            <asp:ListItem Value="'YL14'">WEST PORT TERMINAL (APMC)</asp:ListItem>
                                            <asp:ListItem Value="'YL21','VL21','WL21','XL21'">Pasir Gudang Terminal (PGT)</asp:ListItem>
                                             <asp:ListItem Value="'YL26','VL26','WL26'">West Port (WPT)</asp:ListItem>
                                             <asp:ListItem Value="'YL27','VL27','WL27','MY10630'">Tanjung Bin (TBIN)</asp:ListItem>
                                            <asp:ListItem Value="'YL24','VL24','WL24','XL24'">Shah Alam (SA)</asp:ListItem>
                                            <asp:ListItem Value="'SG10720'">  YTL CEMENT SINGAPORE</asp:ListItem>
                                          
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr id="tpt1">
                                    <td align="left">
                                        <b style="color: #5f7afc;">Transporter</b>
                                    </td>
                                    <td>
                                        <b style="color: #5f7afc;">:</b>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList ID="ddlTransport" runat="server" Font-Size="12px" Width="260px"
                                            Font-Names="verdana">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                         <tr>
									<td align="left">
										<b style="color: #5f7afc;">Ship To</b>
									</td>
									<td>
										<b style="color: #5f7afc;">:</b>
									</td>
									<td align="left">
										<asp:DropDownList ID="ddlGeofence" runat="server" Font-Size="12px" Width="260px" Font-Names="verdana"
											EnableViewState="False">
											
										</asp:DropDownList>
									</td>
								</tr>			
                                     	

                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">Status Types</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <asp:DropDownList runat="server" ID="ddlstatus"  Font-Size="12px" Width="260px" Font-Names="verdana">
                                                     <asp:ListItem Text="ALL STATUS" Value="-1" />
                                                    <asp:ListItem Text="In Progress" Value="0" />
                                                    <asp:ListItem Text="Submitted" Value="1" />
                                                     <asp:ListItem Text="Posted" Value="2" />
                                                     <asp:ListItem Text="Rejected" Value="3" />
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <br />
                                            </td>
                                            <td colspan="2" align="center">
                                                <br />
                                                <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit" ToolTip="Submit" />
                                              <a href="javascript:ExcelReport();" class="button"><span class="ui-button-text" title="Download">
                                            Download</a>


                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td colspan="3">
                    <div style="width: 1190px;">
                        <%If ec = True Then%>
                        <%=sb1.ToString()%>
                        <%End If%>
                    </div>
                </td>
            </tr>
        </table>
    </center>
     
    </form>
     <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="e-POD Report" />
    </form>
     <div class="demo">
         <input type="hidden" name="h1" value="" id="hdnP" />
       <div id="MyDiv" title="View Driver Upload Data" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="driverData" name="driverData" frameborder="0" scrolling="no"
            height="500" width="998px" style="visibility: hidden; border: solid 1px #aac6ff;"/>
    </div>
    </div>
    
    <div class="demo">
       <div id="dialog-message" title="Information" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="mappage" name="mappage" src="" frameborder="0" scrolling="no"
            height="500" width="998px" style="visibility: hidden; border: solid 1px #aac6ff;"
           />
    </div>
    </div>
  
</body>
</html>
<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.orderDeliveryMoniterReport" Codebehind="orderDeliveryMoniterReport.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Delivery Monitoring Report</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        .dataTables_info
        {
            width: 25%;
            float: left;
        }
        .singleline{
            white-space :nowrap;
        }
        .redfont{
            color:red;
        }
    </style>
    <style>
        .hideen {
            visibility: hidden;
            /* margin-top:-80px;*/
        }

        .opacity {
            /* IE 8 */
            -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=30)";
            /* IE 5-7 */
            filter: alpha(opacity=30);
            /* Netscape */
            -moz-opacity: 0.3;
            /* Safari 1.x */
            -khtml-opacity: 0.3;
            opacity: 0.3;
            pointer-events: none;
        }

        #floatingBarsG {
            position: fixed;
            top: 50%;
            left: 50%;
            /*width: 100%;
            height: 100%;

            left: 25%;*/
        }
    </style>



    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">

        function getWindowWidth() { if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement && document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth; }
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }
		
        $(function () {
           

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: getWindowWidth() - 60,
                minHeight: getWindowHeight() - 200
            });

            $("#idlingpage").width(getWindowWidth() - 65);
            $("#idlingpage").height(getWindowHeight() - 200);
      });
             
             function DisplayMap(intime,outtime,plateno) {
             document.getElementById("mappage").src = "GMap.aspx?bdt=" + intime + "&edt=" + outtime + "&plateno="+ plateno +"&scode=1&sf=0&r=" + Math.random();
                 document.getElementById("mappage").style.visibility = "visible";

          $("#dialog-message").dialog("open");  
            }  
    
     
function mysubmit()
{
    var username = document.getElementById("ddltransporter").value;
    if (username=="0")
    {
         alert("Please select transporter name");
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
    $("#reporttype").val("1");
    $("#otrack").val($(".dateline").html());
    $("#tonnage").val($(".tonnage").html());
    $("#rperoid").val($(".dateperiod").html());
    $("#trips").val($(".trips").html());
   
    
        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    
        }
        function openpage(id,bdt,edt,tid) {
            openMap("ShowCompetitorInformation.aspx?geoid=" + id + "&transid="+ tid +"&bdt="+ bdt +"&edt="+ edt);
        }

        function openMap(message) {
            document.getElementById("idlingpage").style.visibility = "visible";
            document.getElementById("idlingpage").src = message;
            $("#dialog-message").dialog("open");
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
            $(document).ready(function () {
		       fnFeaturesInit()
                oTable= $('#examples').dataTable({
		            "bJQueryUI": true,
		            "sPaginationType": "full_numbers",
                    "iDisplayLength": 25,
                   "aLengthMenu": [ 10,25,50,100,200,500 ],
                    "fnDrawCallback": function (oSettings) {
                       if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                   "aoColumnDefs": [
                          {   "bVisible": true, "bSortable": false, "aTargets": [0] } ,
                           
                           
                            
                    ]
                });

              

		          });


    </script>
    
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="Form1" method="post" runat="server">
    <center>
        <div>
            <br />
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Delivery Monitoring Report</b>
            <br />
            <br />
        </div>
        <table>
            <tr>
                <td align="center">
                    <table style="font-family: Verdana; font-size: 11px;" class="maintbl">
                        <tr>
                            <td colspan="2" style="height: 20px; background-color: #465ae8;" align="left">
                                <b style="color: White;">&nbsp;Delivery Monitoring Report&nbsp;:</b>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 450px; border: solid 1px #5B7C97;">
                                <table style="width: 450px;">
                                    <tbody>
                                          <tr>
                                            <td align="left" class="singleline">
                                                <b style="color: #465AE8;">Order</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <select id="ddlorder" class="form-control" name="ddlorder" onchange="GetOrderInfo()" style ="width :255px" runat ="server" ></select>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="singleline">
                                                <b style="color: #465AE8;">Trips Loaded/ Total Order Trip</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" class="trips">
                                               
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="singleline">
                                                <b style="color: #465AE8;">Tonnage Loaded/Order Tonnage</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" class="tonnage">
                                               
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="singleline">
                                                <b style="color: #465AE8;">Hours to Dateline</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" class="dateline" >
                                               
                                            </td>
                                        </tr>
                                         <tr style ="display:none ">
                                            <td align="left" class="singleline">
                                                <b style="color: #465AE8;">Hours to Dateline</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" class="dateperiod" >
                                               
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="singleline">
                                                <b style="color: #465AE8;">Internal Trucks</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" class="internaltruck" style="overflow-wrap: anywhere;max-width: 350px;display: block;">
                                               
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="singleline">
                                                <b style="color: #465AE8;">External Trucks</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" class="externaltruck" style="overflow-wrap: anywhere;max-width: 350px;display: block;">
                                               
                                            </td>
                                        </tr>



                                       
                                        <tr>
                                            <td align="center">
                                                <br />
                                            </td>
                                            <td colspan="2" align="center">
                                                <br />
                                                 <a href="javascript:RefreshData();" class="button"><span class="ui-button-text"
                                                        title="Submit">Refresh</a>
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
                    <div style="width: 100%;">
                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
                                <thead>
                                    <tr>
                                        <th style="width: 20px">SNo</th>
                                        <th style="width: 80px">Username</th>
                                         <th style="width: 50px">Plateno</th>
                                        <th style="width: 50px">Truck Code</th>
                                        <%--<th>Trips</th>--%>
                                         <th>Weightout Datetime</th>
                                        <th>Source</th>
                                        <th>Ship To</th>
                                        <th>Status</th>
                                        <th>DN Qty</th>
                                        <th>Current Location</th>
                                        <th>Distance To Destination</th>
                                        <th>Distance To Plant</th>
                                        <%-- <th>Current Location</th>
                                         <th>Time Since Last Delivery</th>
                                        <th>Distance To Plant</th>
                                        <th>Distance To Destination</th>
                                        <th>Remarks</th>--%>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                    </div>
                </td>
            </tr>
        </table>
    </center>
     <div id="floatingBarsG" class="hideen" style="z-index: 999; opacity: 2;">
            <center>
            <div class="row">

            <div class ="col-md-12">
                  <div class="box box-primary">
                <div class="box-header">
                  <h3 class="box-title">Loading</h3>
                </div>
               <!-- /.box-body -->
                <!-- Loading (remove the following to stop the loading)-->
               <%-- <div class="overlay">
                  <i class="fa fa-refresh fa-spin"></i>
                </div>--%>
                <!-- end loading -->
              </div><!-- /.box -->
            </div>
                   </div>
                </center>
        </div>
    </form>
     <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Delivery Monitoring Report" />
    <input type="hidden" id="plateno" name="plateno" value="" />
          <input type="hidden" id="reporttype" name="reporttype" value="1" />
          <input type="hidden" id="rperoid" name="rperoid" value="" />
         <input type="hidden" id="otrack" name="otrack" value="" />
         <input type="hidden" id="internal" name="internal" value="" />
         <input type="hidden" id="external" name="external" value="" />
         <input type="hidden" id="trips" name="trips" value="" />
         <input type="hidden" id="tonnage" name="tonnage" value="" />
    </form>
    

    <script type="text/javascript" >
        var ec =<%=ec %>;
       
        function RefreshData() {
            GetOrderInfo();
        }
        $(function () {
            $("#floatingBarsG").addClass("hideen");
            $("body").removeClass("opacity");

        });
        function GetOrderInfo() {
            if ($("#ddlorder").val() != 0) {
                $("#floatingBarsG").removeClass("hideen");
                $("body").addClass("opacity");
                $.ajax({
                    type: "POST",
                    url: "orderDeliveryMoniterReport.aspx/GetData",
                    data: '{orderid:\"' + $("#ddlorder").val() + '\"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        var json = response.aaData;

                        $(".trips").html(json[0][0] +"/"+ json[0][1] +" ["+((json[0][0] / json[0][1]) * 100).toFixed(0)+"%]");
                        $(".tonnage").html(json[0][2] +"/"+ json[0][3] +" ["+(((json[0][2] / json[0][3])) * 100).toFixed(0)+"%]");
                        if (json[0][4] > 0) {
                            $(".dateline").html(json[0][4] + " Hours Left");
                            $(".dateline").removeClass("redfont");
                        }
                        else {
                            $(".dateline").html("Exceeded By Hours " + (json[0][4] * -1));
                            $(".dateline").addClass("redfont");
                        }
                        $(".internaltruck").html(json[0][5]);
                        $(".externaltruck").html(json[0][6]);
                        $(".dateperiod").html(json[0][7])
                        

                        //table = oTable.dataTable();
                        //table._fnProcessingDisplay(true);
                        //oSettings = table.fnSettings();
                        //table.fnClearTable(this);
                        //for (var i = 0; i < json.length-1; i++) {
                        //    table.oApi._fnAddData(oSettings, json[i]);
                        //}

                        //oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                        //table._fnProcessingDisplay(false);
                        //table.fnDraw();
                        //ec = true;
                        $("#floatingBarsG").addClass("hideen");
                        $("body").removeClass("opacity");
                        Submit();
                    },
                    failure: function (response) {
                        alert("Error");
                    }
                });
            }
            else {
                alertbox("Please select order");
            }
           
        }

        function Submit() {
            $("#floatingBarsG").removeClass("hideen");
            $("body").addClass("opacity");
            $.ajax({
                type: "POST",
                url: "orderDeliveryMoniterReport.aspx/GetTable",
                data: '{orderid:\"' + $("#ddlorder").val() + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var json = response.aaData;
                   table = oTable.dataTable();
                        table._fnProcessingDisplay(true);
                        oSettings = table.fnSettings();
                        table.fnClearTable(this);
                        for (var i = 0; i < json.length; i++) {
                            table.oApi._fnAddData(oSettings, json[i]);
                        }

                        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                        table._fnProcessingDisplay(false);
                        table.fnDraw();
                    ec = true;
                    $("#floatingBarsG").addClass("hideen");
                    $("body").removeClass("opacity");
                },
                failure: function (response) {
                    alert("Error");
                }
            });
        }
    </script>
   
</body>
</html>
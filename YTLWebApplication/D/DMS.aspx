<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.DMS" Codebehind="DMS.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Delivery Monitoring Summary Daily</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/ColVis.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #fw_container
        {
            width: 100%;
        }
        
        .style1
        {
            height: 18px;
        }
        
        table.display tbody td
        {
            vertical-align: top;
        }
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript" scr="https://www.datatables.net/release-datatables/extensions/ColReorder/js/dataTables.colReorder.js"></script>
    <script type ="text/javascript">
        function ShowrHideFavorite() {
            if ($("#ddlGeofence").val() != "ALL GEOFENCES") {
                if ($("#ddlGeofence option:selected").attr("favorite") == "True") {
                    $(".addf").hide();
                    $(".removef").show();
                }
                else {
                    $(".addf").show();
                    $(".removef").hide();
                }
            }
            else {
                $(".addf").hide();
                $(".removef").hide();
            }


        }


        function AddFavorite() {
            $.ajax({
                type: "POST",
                url: "DMS.aspx/ManageFavorite",
                data: '{geoid: \"' + $('#ddlGeofence').val() + '\",op: \"0\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var json = response;
                    if (json == "1") {
                        alert("Geofence Added To Favorite");
                        window.location.href = "DMS.aspx";
                    }
                    else {
                        alert("Error");
                    }
                },
                failure: function (response) {
                    alert("Error");
                }
            });
        }
        function RemoveFavorite() {
            $.ajax({
                type: "POST",
                url: "DMS.aspx/ManageFavorite",
                data: '{geoid: \"' + $('#ddlGeofence').val() + '\",op: \"1\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var json = response;
                    if (json == "1") {
                        alert("Geofence Removed From Favorite");
                        window.location.href = "DMS.aspx";
                    }
                    else {
                        alert("Error");
                    }
                },
                failure: function (response) {
                    alert("Error");
                }
            });
        }
    </script>
    <script type="text/javascript">
        $(function () {
            $(".addf").hide();
            $(".removef").hide();
            ShowrHideFavorite();
            $("#ddlGeofence").change(function () {
                ShowrHideFavorite();
            });

            $("#ddlbh").val("07");
            $("#ddleh").val("07");
            $("#ddlem").val("00");

            
            

     $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
               minWidth: 1000,
                minHeight: 529
            });

             $("#LogReportDiv").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
               minWidth: 1052,
                minHeight: 410
            });
             $("#ossdiv").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
               minWidth: 1300,
                minHeight: 250
            });
            

             });
        var oTable;
           function openpage(id) {
             openMap("ShowMap.aspx?id=" + id);
        }
        function openMap(message) {
           document.getElementById("idlingpage").style.visibility="visible";
            document.getElementById("idlingpage").src = message;
            $("#dialog-message").dialog("open");
        }

         function openLog(message) {
           document.getElementById("logpage").style.visibility="visible";
            document.getElementById("logpage").src = message;
            $("#LogReportDiv").dialog("open");
        }
         function openoss(message) {
           document.getElementById("ossframe").style.visibility="visible";
            document.getElementById("ossframe").src = message;
            $("#ossdiv").dialog("open");
        }


        function openMapPage(plateno,bdt,edt,dmslat,dmslon) {
    openMap("GMap.aspx?plateno=" + plateno + "&bdt=" + bdt + "&edt=" + edt + "&scode=5"+ "&markerlat=" + dmslat + "&markerlon=" + dmslon);
}
  var ec=<%=ec %>;
  var sentData = false;
        function refreshTable() {
                
            table = oTable.dataTable();
            oSettings = table.fnSettings();
            document.getElementById("examples_processing").style.visibility="";              var bdt=document.getElementById("txtBeginDate").value +" "+ document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value +":00";
               var edt=document.getElementById("txtEndDate").value +" "+ document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value +":59";
               var plateno=document.getElementById("ddlplate").value;
               var transporter=document.getElementById("ddlTransporter").value;
               var geofence=document.getElementById("ddlGeofence").value;
                var userid=document.getElementById("DropDownList1").value;
                   $.getJSON('GetDms.aspx?bdt=' + bdt +'&edt='+ edt +'&plateno='+ plateno +'&transporter='+ transporter +'&geofence='+ geofence +'&userid='+ userid , null, function (json) {
                 table.fnClearTable(this);
                  sentData=true;
                  table.fnSettings().oLanguage.sEmptyTable = "<div style='margin-left:12%;'>No Data for the above selection in selected periods.</div>";
               

                for (var i = 0; i < json.length; i++) {
                    table.oApi._fnAddData(oSettings, json[i]);
                }

                oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                  document.getElementById("examples_processing").style.visibility="hidden";
                table.fnDraw();
                ec=true;
            });
          
        }

        $(document).ready(function () {
             $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -480, maxDate: -1, changeMonth: true, changeYear: true, numberOfMonths: 2
          });

          $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -480, maxDate: -1, changeMonth: true, changeYear: true, numberOfMonths: 2

          });
            

            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "bAutoWidth": false,
                 "bProcessing": true,                "sPaginationType": "full_numbers",
                "iDisplayLength": 5,
               // "sAjaxSource": "GetDmsTest.aspx?bdt=" + document.getElementById("txtBeginDate").value,
                "aLengthMenu": [5,10,20,50, 100, 200, 500],
                "aaSorting": [[0, "asc"]],
                "sScrollX": "260%",
                 "oLanguage": {"sProcessing": " Loading...","sEmptyTable": "<div style='margin-left:15%;'>Please click on Submit button to display data.</div>"
                 },
	         //	"sScrollXInner": "350%",

                "fnDrawCallback": function (oSettings) {
                },
                 aoColumns: [
                     { "bVisible": true, "bSortable": true,"sWidth":"200px", "aTargets": [0] },
                     { "bVisible": true, "bSortable": true, "sWidth":"50px","aTargets": [1] },
                    // { "bVisible": true, "bSortable": true, "sWidth":"40px","aTargets": [2] },

                      { "bVisible": true, "bSortable": true, "sWidth":"60px", "fnRender": function (oData, sVal) {
                                 var status = sVal;
                                 var url="GetOssDetails.aspx?p="+ sVal ;
                                return '<span style="cursor:pointer;Color:Blue;text-decoration:underline;" onclick=\'javascript: openoss(\"'+ url +'\")\' title="Click to View Oss Details" >View</span>';    
                                                          
                         }, "aTargets": [2] },


                    { "bVisible": true, "bSortable": true, "sWidth":"35px","aTargets": [3] },


                     { "bVisible": true, "bSortable": true, "sWidth":"90px","aTargets": [4] },
                     { "bVisible": true, "bSortable": true, "sWidth":"60px","aTargets": [5] },
                     { "bVisible": false, "bSortable": true, "sWidth":"45px","aTargets": [6] },                    
                     { "bVisible": true, "bSortable": true, "sWidth":"45px","aTargets": [7] },
                     { "bVisible": true, "bSortable": true, "sWidth":"60px","aTargets": [8] },
                     { "bVisible": true, "bSortable": true, "sWidth":"50px","aTargets": [9] },
                     { "bVisible": true, "bSortable": true, "sWidth":"60px","aTargets": [10] },
                     { "bVisible": true, "bSortable": true, "sWidth":"90px","aTargets": [11] },
                     { "bVisible": true, "bSortable": true, "sWidth":"90px","aTargets": [12] },
                     { "bVisible": true, "bSortable": true, "sWidth":"60px","aTargets": [13] },
                     { "bVisible": true, "bSortable": true, "sWidth":"75px","aTargets": [14] },
                     { "bVisible": true, "bSortable": true, "sWidth":"60px","aTargets": [15] },
                     { "bVisible": true, "bSortable": true, "sWidth":"60px","aTargets": [16] },
                      { "bVisible": true, "bSortable": true, "sWidth":"130px","aTargets": [17] },
                     { "bVisible": true, "bSortable": true, "sWidth":"50px","aTargets": [18] }, 
                     { "bVisible": true, "bSortable": true, "sWidth":"200px","aTargets": [19] },    
                      { "bVisible": true, "bSortable": true, "sWidth":"145px","aTargets": [20] },
                     { "bVisible": true, "bSortable": true, "sWidth":"50px","aTargets": [21] },
                     { "bVisible": true, "bSortable": true, "sWidth":"130px","aTargets": [22] },  
                     { "bVisible": true, "bSortable": true, "sWidth":"50px","aTargets": [23] },

                      { "bVisible": true, "bSortable": true, "sWidth":"200px","aTargets": [24] },
                     { "bVisible": true, "bSortable": true, "sWidth":"145px","aTargets": [25] },
                     { "bVisible": true, "bSortable": true, "sWidth":"50px","aTargets": [26] },  
                     { "bVisible": true, "bSortable": true, "sWidth":"40px","aTargets": [27] },
                      { "bVisible": true, "bSortable": true, "sWidth":"40px", "fnRender": function (oData, sVal) {
                                 var status = sVal;                            
                                 var userid = oData.aData[29];
                                 var bdtL = oData.aData[30];
                                 var edtL = oData.aData[31];
                                 var url="GetLostData.aspx?p="+ oData.aData[5] +"&b="+ bdtL +"&e=" + edtL +"&userid="+ oData.aData[29] ;
                                if (status == "Yes") {
                                 return '<span style="cursor:pointer;Color:Blue;text-decoration:underline;" onclick=\'javascript: openLog(\"'+ url +'\")\' title="Click to View Log Report" >' + status + '</span>';    
                                }
                             else {
                             return status;
                              }
                            
                         }, "aTargets": [28] },
                          { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[32];                            
                         }, "aTargets": [29] },
                           { "bVisible": true, "bSortable": true, "sWidth":"30px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[33];                            
                         }, "aTargets": [30] },
                         { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[34];                            
                         }, "aTargets": [31] },
                         { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[35];                            
                         }, "aTargets": [32] },

                          { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[36];                            
                         }, "aTargets": [33] },
                          { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[37];                            
                         }, "aTargets": [34] },
                          { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[38];                            
                         }, "aTargets": [35] },

                          { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[39];                            
                         }, "aTargets": [36] },
                          { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[40];                            
                         }, "aTargets": [37] },
                          { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[41];                            
                         }, "aTargets": [38] },
                          { "bVisible": true, "bSortable": true, "sWidth":"140px", "fnRender": function (oData, sVal) {                              
                             return oData.aData[42];                            
                         }, "aTargets": [39] }


                     ]
            });      
             //     settings = oTable.dataTable().fnSettings();
        //   oTable = jQuery("#example").dataTable(settings);
        //   oTable.fnColReorder(2, 38);//move the 4th column on the 10th position
         // oTable.fnAdjustColumnSizing();//a good idea to make sure there will be no displaying issues
             
        });
      
function ExcelReport()
{
    if(ec==true)
    {
      
        var bdt=document.getElementById("txtBeginDate").value +" "+ document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value +":00";
        var edt=document.getElementById("txtEndDate").value +" "+ document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value +":59";
        var plateno=document.getElementById("ddlplate").value;
        document.getElementById("tplateno").value= plateno;
        document.getElementById("tbdt").value=bdt;
         document.getElementById("tedt").value=edt;        
        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    }
    else
    {
        alert("First click submit button");
    }
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
    </script>
    <style type="text/css">
        .dataTables_empty
        {
            text-align: left;
            font-size: 15px;
            color: Blue;
            font-weight: bold;
        }
        .dataTables_processing
        {
            position: absolute;
            top: 8px;
            left: 50%;
            width: 150px;
            margin-left: -125px;
            border: 1px solid #ddd;
            text-align: center;
            color: white;
            font-size: 11px;
            font-weight: bold;
            padding: 2px 0;
            background-color: Black;
            opacity: .7; /*The good stuff */
            -webkit-transition: opacity 0.5s ease-out; /* Saf3.2+, Chrome */
            -moz-transition: opacity 0.5s ease-out; /* FF4+ */
            -ms-transition: opacity 0.5s ease-out; /* IE10? */
            -o-transition: opacity 0.5s ease-out; /* Opera 10.5+ */
            transition: opacity 0.5s ease-out;
        }
    </style>
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="reportform" runat="server">
    <center>
        <br />
        <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Delivery Monitoring
            Summary Daily</b>
        <br />
        <div style="position: absolute; left: 10px; top: 35px; text-align: left; height: 267px;">
            <span style="color: navy; font-family: Verdana; font-size: 11px; font-weight: normal;">
                <b>Data Lost (And) V Data </b>
                <br />
                <br />
                1.Data Invalid (V) more than 15 minutes
                <br />
                <br />
                2. Data Lost with Ignition OFF more than 2 hours<br />
                <br />
                3. Data Lost with Ignition ON more than 15 minutes<br>
                <br />
            </span>
        </div>
        <br />
        <table style="font-family: Verdana; font-size: 11px;">
            <tr>
                <td colspan="2" style="height: 20px; background-color: #465ae8;" align="left">
                    <b style="color: White;">&nbsp;Delivery Monitoring Summary Daily&nbsp;:</b>
                </td>
            </tr>
            <tr>
                <td style="width: 450px; border: solid 1px #5B7C97;">
                    <table style="width: 450px;">
                        <tbody>
                            <tr>
                                <td align="left" style="width: 290px;">
                                    <b style="color: #465AE8;">Weight Out Begin</b>
                                </td>
                                <td>
                                    <b style="color: #465AE8;">:</b>
                                </td>
                                <td align="left" style="width: 326px">
                                    <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                        id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" /><b
                                            style="color: #465AE8;">&nbsp;Hour&nbsp;:&nbsp;
                                            <asp:DropDownList ID="ddlbh" runat="server" Width="40px" EnableViewState="False">
                                                <asp:ListItem Value="00">00</asp:ListItem>
                                                <asp:ListItem Value="01">01</asp:ListItem>
                                                <asp:ListItem Value="02">02</asp:ListItem>
                                                <asp:ListItem Value="03">03</asp:ListItem>
                                                <asp:ListItem Value="04">04</asp:ListItem>
                                                <asp:ListItem Value="05">05</asp:ListItem>
                                                <asp:ListItem Value="06">06</asp:ListItem>
                                                <asp:ListItem Value="07">07</asp:ListItem>
                                                <asp:ListItem Value="08">08</asp:ListItem>
                                                <asp:ListItem Value="09">09</asp:ListItem>
                                                <asp:ListItem Value="10">10</asp:ListItem>
                                                <asp:ListItem Value="11">11</asp:ListItem>
                                                <asp:ListItem Value="12">12</asp:ListItem>
                                                <asp:ListItem Value="13">13</asp:ListItem>
                                                <asp:ListItem Value="14">14</asp:ListItem>
                                                <asp:ListItem Value="15">15</asp:ListItem>
                                                <asp:ListItem Value="16">16</asp:ListItem>
                                                <asp:ListItem Value="17">17</asp:ListItem>
                                                <asp:ListItem Value="18">18</asp:ListItem>
                                                <asp:ListItem Value="19">19</asp:ListItem>
                                                <asp:ListItem Value="20">20</asp:ListItem>
                                                <asp:ListItem Value="21">21</asp:ListItem>
                                                <asp:ListItem Value="22">22</asp:ListItem>
                                                <asp:ListItem Value="23">23</asp:ListItem>
                                            </asp:DropDownList>
                                        </b><b style="color: #465AE8;">&nbsp;Min&nbsp;:&nbsp;
                                            <asp:DropDownList ID="ddlbm" runat="server" Width="40px" EnableViewState="False">
                                                <asp:ListItem Value="00">00</asp:ListItem>
                                                <asp:ListItem Value="01">01</asp:ListItem>
                                                <asp:ListItem Value="02">02</asp:ListItem>
                                                <asp:ListItem Value="03">03</asp:ListItem>
                                                <asp:ListItem Value="04">04</asp:ListItem>
                                                <asp:ListItem Value="05">05</asp:ListItem>
                                                <asp:ListItem Value="06">06</asp:ListItem>
                                                <asp:ListItem Value="07">07</asp:ListItem>
                                                <asp:ListItem Value="08">08</asp:ListItem>
                                                <asp:ListItem Value="09">09</asp:ListItem>
                                                <asp:ListItem Value="10">10</asp:ListItem>
                                                <asp:ListItem Value="11">11</asp:ListItem>
                                                <asp:ListItem Value="12">12</asp:ListItem>
                                                <asp:ListItem Value="13">13</asp:ListItem>
                                                <asp:ListItem Value="14">14</asp:ListItem>
                                                <asp:ListItem Value="15">15</asp:ListItem>
                                                <asp:ListItem Value="16">16</asp:ListItem>
                                                <asp:ListItem Value="17">17</asp:ListItem>
                                                <asp:ListItem Value="18">18</asp:ListItem>
                                                <asp:ListItem Value="19">19</asp:ListItem>
                                                <asp:ListItem Value="20">20</asp:ListItem>
                                                <asp:ListItem Value="21">21</asp:ListItem>
                                                <asp:ListItem Value="22">22</asp:ListItem>
                                                <asp:ListItem Value="23">23</asp:ListItem>
                                                <asp:ListItem Value="24">24</asp:ListItem>
                                                <asp:ListItem Value="25">25</asp:ListItem>
                                                <asp:ListItem Value="26">26</asp:ListItem>
                                                <asp:ListItem Value="27">27</asp:ListItem>
                                                <asp:ListItem Value="28">28</asp:ListItem>
                                                <asp:ListItem Value="29">29</asp:ListItem>
                                                <asp:ListItem Value="30">30</asp:ListItem>
                                                <asp:ListItem Value="31">31</asp:ListItem>
                                                <asp:ListItem Value="32">32</asp:ListItem>
                                                <asp:ListItem Value="33">33</asp:ListItem>
                                                <asp:ListItem Value="34">34</asp:ListItem>
                                                <asp:ListItem Value="35">35</asp:ListItem>
                                                <asp:ListItem Value="36">36</asp:ListItem>
                                                <asp:ListItem Value="37">37</asp:ListItem>
                                                <asp:ListItem Value="38">38</asp:ListItem>
                                                <asp:ListItem Value="39">39</asp:ListItem>
                                                <asp:ListItem Value="40">40</asp:ListItem>
                                                <asp:ListItem Value="41">41</asp:ListItem>
                                                <asp:ListItem Value="42">42</asp:ListItem>
                                                <asp:ListItem Value="43">43</asp:ListItem>
                                                <asp:ListItem Value="44">44</asp:ListItem>
                                                <asp:ListItem Value="45">45</asp:ListItem>
                                                <asp:ListItem Value="46">46</asp:ListItem>
                                                <asp:ListItem Value="47">47</asp:ListItem>
                                                <asp:ListItem Value="48">48</asp:ListItem>
                                                <asp:ListItem Value="49">49</asp:ListItem>
                                                <asp:ListItem Value="50">50</asp:ListItem>
                                                <asp:ListItem Value="51">51</asp:ListItem>
                                                <asp:ListItem Value="52">52</asp:ListItem>
                                                <asp:ListItem Value="53">53</asp:ListItem>
                                                <asp:ListItem Value="54">54</asp:ListItem>
                                                <asp:ListItem Value="55">55</asp:ListItem>
                                                <asp:ListItem Value="56">56</asp:ListItem>
                                                <asp:ListItem Value="57">57</asp:ListItem>
                                                <asp:ListItem Value="58">58</asp:ListItem>
                                                <asp:ListItem Value="59">59</asp:ListItem>
                                            </asp:DropDownList>
                                        </b>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b style="color: #465AE8;">Weight Out End</b>
                                </td>
                                <td>
                                    <b style="color: #465AE8;">:</b>
                                </td>
                                <td align="left" style="width: 326px">
                                    <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                        id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" /><b style="color: #465AE8;">&nbsp;Hour&nbsp;:&nbsp;
                                            <asp:DropDownList ID="ddleh" runat="server" Width="40px" EnableViewState="False">
                                                <asp:ListItem Value="00">00</asp:ListItem>
                                                <asp:ListItem Value="01">01</asp:ListItem>
                                                <asp:ListItem Value="02">02</asp:ListItem>
                                                <asp:ListItem Value="03">03</asp:ListItem>
                                                <asp:ListItem Value="04">04</asp:ListItem>
                                                <asp:ListItem Value="05">05</asp:ListItem>
                                                <asp:ListItem Value="06">06</asp:ListItem>
                                                <asp:ListItem Value="07">07</asp:ListItem>
                                                <asp:ListItem Value="08">08</asp:ListItem>
                                                <asp:ListItem Value="09">09</asp:ListItem>
                                                <asp:ListItem Value="10">10</asp:ListItem>
                                                <asp:ListItem Value="11">11</asp:ListItem>
                                                <asp:ListItem Value="12">12</asp:ListItem>
                                                <asp:ListItem Value="13">13</asp:ListItem>
                                                <asp:ListItem Value="14">14</asp:ListItem>
                                                <asp:ListItem Value="15">15</asp:ListItem>
                                                <asp:ListItem Value="16">16</asp:ListItem>
                                                <asp:ListItem Value="17">17</asp:ListItem>
                                                <asp:ListItem Value="18">18</asp:ListItem>
                                                <asp:ListItem Value="19">19</asp:ListItem>
                                                <asp:ListItem Value="20">20</asp:ListItem>
                                                <asp:ListItem Value="21">21</asp:ListItem>
                                                <asp:ListItem Value="22">22</asp:ListItem>
                                                <asp:ListItem Value="23" Selected="True">23</asp:ListItem>
                                            </asp:DropDownList>
                                        </b><b style="color: #465AE8;">&nbsp;Min&nbsp;:&nbsp;
                                            <asp:DropDownList ID="ddlem" runat="server" Width="40px" EnableViewState="False">
                                                <asp:ListItem Value="00">00</asp:ListItem>
                                                <asp:ListItem Value="01">01</asp:ListItem>
                                                <asp:ListItem Value="02">02</asp:ListItem>
                                                <asp:ListItem Value="03">03</asp:ListItem>
                                                <asp:ListItem Value="04">04</asp:ListItem>
                                                <asp:ListItem Value="05">05</asp:ListItem>
                                                <asp:ListItem Value="06">06</asp:ListItem>
                                                <asp:ListItem Value="07">07</asp:ListItem>
                                                <asp:ListItem Value="08">08</asp:ListItem>
                                                <asp:ListItem Value="09">09</asp:ListItem>
                                                <asp:ListItem Value="10">10</asp:ListItem>
                                                <asp:ListItem Value="11">11</asp:ListItem>
                                                <asp:ListItem Value="12">12</asp:ListItem>
                                                <asp:ListItem Value="13">13</asp:ListItem>
                                                <asp:ListItem Value="14">14</asp:ListItem>
                                                <asp:ListItem Value="15">15</asp:ListItem>
                                                <asp:ListItem Value="16">16</asp:ListItem>
                                                <asp:ListItem Value="17">17</asp:ListItem>
                                                <asp:ListItem Value="18">18</asp:ListItem>
                                                <asp:ListItem Value="19">19</asp:ListItem>
                                                <asp:ListItem Value="20">20</asp:ListItem>
                                                <asp:ListItem Value="21">21</asp:ListItem>
                                                <asp:ListItem Value="22">22</asp:ListItem>
                                                <asp:ListItem Value="23">23</asp:ListItem>
                                                <asp:ListItem Value="24">24</asp:ListItem>
                                                <asp:ListItem Value="25">25</asp:ListItem>
                                                <asp:ListItem Value="26">26</asp:ListItem>
                                                <asp:ListItem Value="27">27</asp:ListItem>
                                                <asp:ListItem Value="28">28</asp:ListItem>
                                                <asp:ListItem Value="29">29</asp:ListItem>
                                                <asp:ListItem Value="30">30</asp:ListItem>
                                                <asp:ListItem Value="31">31</asp:ListItem>
                                                <asp:ListItem Value="32">32</asp:ListItem>
                                                <asp:ListItem Value="33">33</asp:ListItem>
                                                <asp:ListItem Value="34">34</asp:ListItem>
                                                <asp:ListItem Value="35">35</asp:ListItem>
                                                <asp:ListItem Value="36">36</asp:ListItem>
                                                <asp:ListItem Value="37">37</asp:ListItem>
                                                <asp:ListItem Value="38">38</asp:ListItem>
                                                <asp:ListItem Value="39">39</asp:ListItem>
                                                <asp:ListItem Value="40">40</asp:ListItem>
                                                <asp:ListItem Value="41">41</asp:ListItem>
                                                <asp:ListItem Value="42">42</asp:ListItem>
                                                <asp:ListItem Value="43">43</asp:ListItem>
                                                <asp:ListItem Value="44">44</asp:ListItem>
                                                <asp:ListItem Value="45">45</asp:ListItem>
                                                <asp:ListItem Value="46">46</asp:ListItem>
                                                <asp:ListItem Value="47">47</asp:ListItem>
                                                <asp:ListItem Value="48">48</asp:ListItem>
                                                <asp:ListItem Value="49">49</asp:ListItem>
                                                <asp:ListItem Value="50">50</asp:ListItem>
                                                <asp:ListItem Value="51">51</asp:ListItem>
                                                <asp:ListItem Value="52">52</asp:ListItem>
                                                <asp:ListItem Value="53">53</asp:ListItem>
                                                <asp:ListItem Value="54">54</asp:ListItem>
                                                <asp:ListItem Value="55">55</asp:ListItem>
                                                <asp:ListItem Value="56">56</asp:ListItem>
                                                <asp:ListItem Value="57">57</asp:ListItem>
                                                <asp:ListItem Value="58">58</asp:ListItem>
                                                <asp:ListItem Value="59" Selected="True">59</asp:ListItem>
                                            </asp:DropDownList>
                                        </b>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b style="color: #465AE8;">User Name</b>
                                </td>
                                <td>
                                    <b style="color: #465AE8;">:</b>
                                </td>
                                <td align="left" style="width: 326px">
                                    <asp:DropDownList ID="DropDownList1" runat="server" Width="248px" AutoPostBack="true">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b style="color: #465AE8;">Plate No </b>
                                </td>
                                <td>
                                    <b style="color: #465AE8;">:</b>
                                </td>
                                <td align="left" style="width: 326px">
                                    <asp:DropDownList ID="ddlplate" runat="server" Width="248px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr style="display: none;">
                                <td align="left">
                                    <b style="color: #465AE8;">Transporter</b>
                                </td>
                                <td>
                                    <b style="color: #465AE8;">:</b>
                                </td>
                                <td align="left" style="width: 326px">
                                    <asp:DropDownList ID="ddlTransporter" runat="server" Width="248px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b style="color: #465AE8;">SHIP To NAME [ CODE ]</b>
                                </td>
                                <td>
                                    <b style="color: #465AE8;">:</b>
                                </td>
                                <td align="left" style="width: 326px">
                                    <asp:DropDownList ID="ddlGeofence" runat="server" Width="248px">
                                    </asp:DropDownList>
                                     <img class="addf" onclick="AddFavorite()"  title="Add To Favorite" style="cursor:pointer;height:25px;width:25px;" src="images/star.png"  />
            <img class="removef" onclick="RemoveFavorite()" title="Remove From Favorite" style="cursor:pointer" src="images/cross_tick.gif" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <br />
                                </td>
                                <td colspan="2" align="center">
                                    <br />
                                    <a href="javascript:refreshTable();" class="button"><span class="ui-button-text"
                                        title="Submit">Submit</span> </a><a href="javascript:ExcelReport();" class="button">
                                            <span class="ui-button-text" title="Download">Download</span> </a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </table>
    </center>
    <br />
    <div id="fw_container">
        <table width="100%" cellpadding="0" cellspacing="0" border="0" class="display" id="examples"
            style="font-size: 10px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <thead>
                <tr align="left">
                    <th rowspan="2">
                        Customer/Ship To Name
                    </th>
                    <th rowspan="2">
                        Ship To Code
                    </th>
                    <th rowspan="2">
                        View Oss
                    </th>
                    <th rowspan="2">
                        EX
                    </th>
                    <th rowspan="2">
                        Transporter Name
                    </th>
                    <th rowspan="2">
                        Vehicle No
                    </th>
                    <th rowspan="2">
                        MT
                    </th>
                    <th rowspan="2">
                        DN No
                    </th>
                    <th colspan="2" style='background-color: #EAF4FD; text-align: center; color: #2E6E9E;'>
                        Out Weight Bridge
                    </th>
                    <th rowspan="2">
                        Journey to Cust
                    </th>
                    <th rowspan="2">
                        ATA
                    </th>
                    <th rowspan="2">
                        ATD
                    </th>
                    <th rowspan="2">
                        Time Spent at Site
                    </th>
                    <th rowspan="2">
                        Back to Source
                    </th>
                    <th rowspan="2">
                        PTO On Time
                    </th>
                    <th rowspan="2">
                        PTO Off Time
                    </th>
                    <th colspan="5" style='background-color: #EAF4FD; text-align: center; color: #2E6E9E;'>
                        Journey Trail - Plant to Customer
                    </th>
                    <th colspan="5" style='background-color: #EAF4FD; text-align: center; color: #2E6E9E;'>
                        Journey Back - Customer to Plant
                    </th>
                    <th rowspan="2">
                        Map
                    </th>
                    <th rowspan="2">
                        Data Lost And V-Data
                    </th>
                    <th rowspan="2">
                        Driver
                    </th>
                    <th rowspan="2">
                        DN Qty
                    </th>
                    <th rowspan="2">
                        Travelling {Mins}
                    </th>
                    <th rowspan="2">
                       Distance
                    </th>
                    <th colspan="3" style='background-color: #EAF4FD; text-align: center; color: #2E6E9E;'>
                       Loading 
                    </th>                    
                    <th colspan="3" style='background-color: #EAF4FD; text-align: center; color: #2E6E9E;'>
                       Waiting 
                    </th> 
                    <th rowspan="2">
                        Unloading {Mins}
                    </th>
                </tr>
                <tr align="left">
                    <th>
                        Date
                    </th>
                    <th>
                        Time
                    </th>
                    <th>
                        Idling/Stop &gt 15 mins
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        Geofence Location
                    </th>
                    <th>
                        PTO
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        Idling/Stop &gt 15 mins
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        Geofence Location
                    </th>
                    <th>
                        PTO
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        IN Time
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        OUT Time
                    </th>
                    <th>
                        IN Time
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        OUT Time
                    </th>
                </tr>
            </thead>
            <tbody>
            </tbody>
            <tfoot>
                <tr align="left">
                    <th rowspan="2">
                        Customer/Ship To Name
                    </th>
                    <th rowspan="2">
                        Ship To Code
                    </th>
                    <th rowspan="2">
                        View Oss
                    </th>
                    <th rowspan="2">
                        EX
                    </th>
                    <th rowspan="2">
                        Transporter Name
                    </th>
                    <th rowspan="2">
                        Vehicle No
                    </th>
                    <th rowspan="2">
                        MT
                    </th>
                    <th rowspan="2">
                        DN No
                    </th>
                    <th>
                        Date
                    </th>
                    <th>
                        Time
                    </th>
                    <th rowspan="2">
                        Journey to Cust
                    </th>
                    <th rowspan="2">
                        ATA
                    </th>
                    <th rowspan="2">
                        ATD
                    </th>
                    <th rowspan="2">
                        Time Spent at Site
                    </th>
                    <th rowspan="2">
                        Back to Source
                    </th>
                    <th rowspan="2">
                        PTO On Time
                    </th>
                    <th rowspan="2">
                        PTO Off Time
                    </th>
                    <th>
                        Idling/Stop &gt 15 mins
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        Geofence Location
                    </th>
                    <th>
                        PTO
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        Idling/Stop &gt 15 mins
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        Geofence Location
                    </th>
                    <th>
                        PTO
                    </th>
                    <th>
                        Duration
                    </th>
                    <th rowspan="2">
                        Map
                    </th>
                    <th rowspan="2">
                        Data Lost And V-Data
                    </th>
                    <th rowspan="2">
                        Driver
                    </th>
                     <th rowspan="2">
                        DN Qty
                    </th>
                     <th rowspan="2">
                        Travelling {Mins}
                    </th>
                    <th rowspan="2">
                       Distance
                    </th>
                    <th>
                        IN Time
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        OUT Time
                    </th>
                   <th>
                        IN Time
                    </th>
                    <th>
                        Duration
                    </th>
                    <th>
                        OUT Time
                    </th>
                    <th rowspan="2">
                        Unloading {Mins}
                    </th>
                </tr>
                <tr align="left">
                    <th colspan="2" class="style1">
                        Out Weight Bridge
                    </th>
                    <th colspan="5" class="style1">
                        Journey Trail - Plant to Customer
                    </th>
                    <th colspan="5" class="style1">
                        Journey Back - Customer to Plant
                    </th>
                       <th colspan="3" class="style1">
                       Loading 
                    </th>  
                     <th colspan="3" class="style1">
                       Waiting 
                    </th>    
                </tr>
            </tfoot>
        </table>
    </div>
    </form>
    <form id="excelform" method="get" action="ExcelDMS.aspx">
    <input type="hidden" id="title" name="title" value="Daily Monitoring Summary Report" />
    <input type="hidden" id="tplateno" name="tplateno" value="" />
    <input type="hidden" id="tbdt" name="tbdt" value="" />
    <input type="hidden" id="tedt" name="tedt" value="" />
    </form>
    <div id="dialog-message" title="Vehicle's Information" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="idlingpage" name="idlingpage" src="" frameborder="0" scrolling="auto"
            height="512" width="998px" style="visibility: hidden; border: solid 1px #aac6ff;">
        </iframe>
    </div>
    <div id="LogReportDiv" title="Detailed Information" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="logpage" name="logpage" src="" frameborder="0" scrolling="auto" height="400px"
            width="1050px" style="visibility: hidden; border: solid 1px #aac6ff;"></iframe>
    </div>
    <div id="ossdiv" title="OSS Information" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="ossframe" name="ossframe" src="" frameborder="0" scrolling="auto" height="250px"
            width="1300px" style="visibility: hidden; border: solid 1px #aac6ff;"></iframe>
    </div>
</body>
</html>

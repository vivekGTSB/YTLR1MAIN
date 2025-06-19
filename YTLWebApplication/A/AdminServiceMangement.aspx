<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.AdminServiceMangement" Codebehind="AdminServiceMangement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Service Management</title>
   <style type="text/css" title="currentStyle">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/jquery-ui.css";
        @import "cssfiles/jquery.ui.dialog.css";
        @import "cssfiles/TableTools.css";
        @import "cssfiles/ColVis.css";
     </style>
   
     <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
     <script type="text/javascript" src="jsfiles/jquery-ui.js"></script>
     <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
     <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
     <link href="cssfiles/style.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/demos22.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/jquery.ui.dialog.css" rel="stylesheet" type="text/css" /> 
     <script type="text/javascript" >
        function refreshTable() {
             table = oTable.dataTable();
             table._fnProcessingDisplay(true);
             oSettings = table.fnSettings();
             $.getJSON('ServiceingJsonT1.aspx?u=' + $("#hidloginuser").val() + '&role=' + $("#hidrole").val() + '&opr=0&r=' + Math.random(), null, function (json) {
                 table.fnClearTable(this);
                 for (var i = 0; i < json.aaData.length; i++) {
                     table.oApi._fnAddData(oSettings, json.aaData[i]);
                 }
                 oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                 table._fnProcessingDisplay(false);
                 table.fnDraw();
             });
             return false;
         }



         $(document).ready(function () {
             $("#txtserdate").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0
             });
             $("#txtservicecompdate").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -30, maxDate: 0
             });
             jQuery.extend(jQuery.fn.dataTableExt.oSort, {
                 "num-html-pre": function (a) {
                     a = $(a).text().substr(2);
                     var x = parseFloat(a.toString().replace(/<.*?>/g, ""));
                     if (isNaN(x)) {
                         if (a.toString().indexOf("fuel sensor problem") > 0) {
                             return -1;
                         }
                         else if (a.toString().indexOf("no fuel sensor") > 0) {
                             return -0.1;
                         }

                         return 0.0;
                     }
                     else {
                         return x;
                     }
                 },

                 "num-html-asc": function (a, b) {
                     return ((a < b) ? -1 : ((a > b) ? 1 : 0));
                 },

                 "num-html-desc": function (a, b) {
                     return ((a < b) ? 1 : ((a > b) ? -1 : 0));
                 }
             });


             oTable = $('#example').dataTable({
                 "bJQueryUI": true,
                 "sPaginationType": "full_numbers",
                 "bPaginate": true,
                 "iDisplayLength": 25,
                 "aaSorting": [[2, "desc"]],
                 "aLengthMenu": [10, 25, 50, 100, 200, 500],
                 "fnDrawCallback": function (oSettings) {
                     if (oSettings.bSorted || oSettings.bFiltered) {
                         if (oSettings.aoColumns[0].bVisible == true) {
                             for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                 $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                             }
                         }
                     }
                 },

                 "aoColumnDefs": [{ "sClass": "left", "bVisible": true, "sWidth": "30px", "bSortable": false, "aTargets": [0] },
                 { "sClass": "left", "bVisible": true, "sWidth": "100px", "bSortable": true, "aTargets": [1] },
                 { "sClass": "left", "aTargets": [2], "sType": "num-html", "bSortable": true, "sWidth": "120px", "asSorting": ["desc", "asc"], "fnRender": function (oData, sVal) {
                     if (oData.aData[17] == 0)
                         return "<span style='cursor:pointer;color:blue;' onclick='javascript:Getticketdetails(\"" + oData.aData[2] + "\")'>" + oData.aData[2] + "</span>";
                     else
                         return "<span style='cursor:pointer;color:blue;' onclick='javascript:Getticketdetails(\"" + oData.aData[2] + "\")'>" + oData.aData[2] + "</span><div class='badge'>" + oData.aData[17] + "</div>";
                 }
                 },
                 { "sClass": "left", "bVisible": true, "sWidth": "180px", "bSortable": true, "aTargets": [3] },
                 { "sClass": "left", "bVisible": true, "sWidth": "180px", "bSortable": true, "aTargets": [4] },
                 { "sClass": "left", "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [5] },
                 { "sClass": "left", "bVisible": true, "sWidth": "50px", "bSortable": true, "aTargets": [6] },
                 { "sClass": "left", "bVisible": true, "sWidth": "120px", "bSortable": true, "aTargets": [7] },
                  { "sClass": "left", "bVisible": true, "sWidth": "120px", "bSortable": true, "aTargets": [8] },
                  { "sClass": "left", "bVisible": true, "sWidth": "120px", "bSortable": true, "aTargets": [9] },
                 //                  { "sClass": "left", "aTargets": [8], "sType": "html", "bSortable": true, "sWidth": "50px", "fnRender": function (oData, sVal) {
                 //                         return oData.aData[9];
                 //                 }
                 //                 },
                 {"sClass": "left", "aTargets": [10], "sType": "html", "bSortable": true, "sWidth": "50px", "fnRender": function (oData, sVal) {
                     if (oData.aData[9] == 'Open') {
                         return "<span style='cursor:pointer;color:blue;' onclick='javascript:Getticketdetails(\"" + oData.aData[12] + "\")'>Assign</span>";
                     }
                     else {
                         return "<span style='cursor:pointer;color:blue;' onclick='javascript:Getticketdetails(\"" + oData.aData[12] + "\")'>View</span>";
                     }
                 }
             }
               ]

         });

         $('#example tbody').on('click', 'tr', function () {
             if ($(this).hasClass('selected')) {
                 $(this).removeClass('selected');
             }
             else {
                 table.$('tr.selected').removeClass('selected');
                 $(this).addClass('selected');
             }
         });

         $("#dialog-message").dialog({
             resizable: false,
             draggable: false,
             modal: true,
             autoOpen: false,
             width: 510,
             minHeight: 300,
             height: 300,
             title: "Assign Service",
             buttons: {
                 Save: function () {
                     serviceassign();
                 },
                 Close: function () {
                     $(this).dialog("close");
                 }
             }
         });
         $("#div-date").dialog({
             resizable: false,
             draggable: false,
             modal: true,
             autoOpen: false,
             width: 300,
             minHeight: 170,
             height: 170,
             title: "Service Date",
             buttons: {
                 Save: function () {
                     changeservicedate();
                 },
                 Close: function () {
                     $(this).dialog("close");
                 }
             }
         });

         $("#dialog-close").dialog({
             resizable: false,
             draggable: false,
             modal: true,
             autoOpen: false,
             width: 510,
             minHeight: 300,
             height: 300,
             title: "Service Close",
             buttons: {
                 Save: function () {
                     closeservice();
                 },
                 Cancel: function () {
                     $(this).dialog("close");
                 }
             }
         });

         $("#div-ticket").dialog({
             resizable: false,
             draggable: false,
             modal: true,
             autoOpen: false,
             width: 570,
             minHeight: 600,
             height: 600,
             title: "Service Infromation",
             buttons: {
                 Close: function () {
                     refreshTable();
                     $(this).dialog("close");
                 }
             }
         });

         $("#dialog-confirm").dialog({
             resizable: false,
             draggable: false,
             height: 150,
             modal: true,
             autoOpen: false,
             buttons: {
                 "Yes": function () {
                     confirmresult = true;
                     $(this).dialog("close");
                 },
                 No: function () {
                     confirmresult = false;
                     $(this).dialog("close");

                 }
             }
         });

         $("#dialog:ui-dialog").dialog("destroy");

         $("#dialog-alert").dialog({
             resizable: false,
             draggable: false,
             modal: true,
             autoOpen: false,
             buttons: {
                 Ok: function () {
                     $(this).dialog("close");
                 }
             }
         });

         refreshTable();
     });

         var oTable;
         var aPos;
         $("#example  tbody td").live("click", function (event) {
             oTable = $('#example').dataTable();
             aPos = oTable.fnGetPosition(this)[0]; // getting the clicked row position
         });
            


         function Assignservice() {
             $("#dialog-message").dialog("open");
         }
         function serviceassign() {
             var assign = $("#txtaasign").val().trim();
             if (assign == "") {
                 alertbox("Enter Name of Person to Assign");
             }
             else {
                 $.getJSON('ServiceingJsonT1.aspx?serviceid=' + $("#serviceid").val() + '&asto=' + $("#txtaasign").val() + '&remarks=' + $("#txtdescri").val() + '&opr=5&r=' + Math.random(), null, function (json) {
                     if (json.d == 1) {
                         $("#dialog-message").dialog("close");

                         Getticketdetails($("#serviceid").val());
                         var ss = "<span style='cursor:pointer;color:blue;' onclick='javascript: Getticketdetails(\"" + $("#serviceid").val() + "\")'>View</span>";

                         var tbl = $('#example').dataTable();
                         var oCell = tbl[0].rows[aPos + 1].cells[8]
                         oCell.innerHTML = "";
                         oCell.innerHTML = ss.toString();
                         oTable.fnUpdate("Technician Assign", aPos, 7);
                     }
                 });
             }
          }
          function Getticketdetails(serviceid) {
             $("#serviceid").val(serviceid);
             $.getJSON('ServiceingJsonT1.aspx?serviceid=' + serviceid + '&u=' + $("#hidloginuser").val() + '&opr=2&r=' + Math.random(), null, function (json) {
                 if (json.length == 1) {
                     $("#plateno").text(json[0][1]);
                     $("#status").text(json[0][5]);
                     if (json[0][7] == null || json[0][5] != "Closed")
                       $("#tdcloseddt").text("");
                     else
                         $("#tdcloseddt").text(json[0][7]);
                     if (json[0][5] == "Open" || json[0][5] == "ReOpen") {
                         $("#assigned").empty();
                         $("#assigned").append("<a class='button' onclick='Assignservice()'  >Assign</a>");
                         $("#btnclose").show();
                         $("#btnsrdt").hide();
                         $("#btnreq").show();
                         $("#btnreopen").hide();
                     }
                     else if (json[0][5] == "Request Received" ) {
                         $("#assigned").empty();
                         $("#assigned").append("<a class='button' onclick='Assignservice()'>Assign</a>");
                         $("#btnclose").show();
                         $("#btnsrdt").hide();
                         $("#btnreq").hide();
                          $("#btnreopen").hide();
                    
                     }
                     else if (json[0][5] == "Technician Assign") {
                         $("#assigned").text(json[0][6]);
                         $("#btnclose").show();
                         $("#btnsrdt").show();
                         $("#btnreq").hide();
                         $("#btnreopen").hide();
                      }

                     
                     else if (json[0][5].indexOf("Service Date Changed")>=0) {
                         $("#assigned").text(json[0][6]);
                         $("#btnclose").show();
                         $("#btnsrdt").hide();
                         $("#btnreq").hide();
                          $("#btnreopen").hide();
                     }
                     else if (json[0][5] == "Closed") {
                         $("#assigned").text(json[0][6]);
                         $("#btnclose").hide();
                         $("#btnsrdt").hide();
                         $("#btnreq").hide();
                          $("#btnreopen").show();
                     }
                     else
                     $("#assigned").text(json[0][6]);

                     $("#priority").text(json[0][4]);
                     if (json[0][2] == null)
                     $("#datetime").text("");
                     else
                         $("#datetime").text(json[0][2]);
                     $("#tdDescription").html("<p>" + json[0][8] + "</P>");
                     if (json[0][5] == "Closed") {
                         $("#txtcomment").attr("disabled", "disabled");
                         $("#btnsend").attr("disabled", "disabled");
                     }
                     else {
                         $("#txtcomment").removeAttr("disabled");
                         $("#btnsend").removeAttr("disabled");
                     }
                     GetComments(0);
                 }
             });



             refreshTable();
             $("#div-ticket").dialog("open");

         }

         function sendcomment() {
             var cmt = $("#txtcomment").val().trim();
             if (cmt == "") {
                 alertbox("Type Comment to Send ");
             }
             else {
                 $.getJSON('ServiceingJsonT1.aspx?serviceid=' + $("#serviceid").val() + '&u=' + $("#hidloginuser").val() + '&Comment=' + $("#txtcomment").val() + '&opr=3&r=' + Math.random(), null, function (json) {
                     if (json.d == 1) {
                         $("#txtcomment").val("");
                         GetComments(0)
                     }
                 });
             }
         }

         function GetComments(st) {
             $.getJSON('ServiceingJsonT1.aspx?serviceid=' + $("#serviceid").val() + '&st=' + st + '&opr=4&r=' + Math.random(), null, function (json) {
                 if (json.Comments.length>= 1) {
                     $("#ullist").empty();
                     $("#ullist").append(json.Comments);
                 }
                 else
                 {
                     $("#ullist").empty();
                 }
             });
         }

         function confirmbox(confirmMessage) {
             confirmresult = false;
             document.getElementById("displayc").innerHTML = confirmMessage;
             $("#dialog-confirm").dialog("open");
         }
         function alertbox(message) {
             document.getElementById("displayp").innerHTML = message;
             $("#dialog-alert").dialog("open");
         }


         function openCloseDialog() {
             $("#txtservicecompdate").val($.datepicker.formatDate("yy/mm/dd", new Date()));
             $("#txtservicecompremarks").val("");
             $("#dialog-close").dialog("open");
         }



         function closeservice() {
             var cmt = $("#txtservicecompremarks").val().trim();
             if (cmt == "") {
                 alertbox("Please Enter Remarks about Service");
             }
             else {
                 confirmbox("Are you sure you want to Close the Service?");
                 $("#dialog-confirm").dialog({
                     close: function (event, ui) {
                         if (confirmresult) {
                             $.getJSON('ServiceingJsonT1.aspx?serviceid=' + $("#serviceid").val() + '&u=' + $("#hidloginuser").val() + '&date=' + $("#txtservicecompdate").val() + '&remarks=' + $("#txtservicecompremarks").val() + '&opr=6&r=' + Math.random(), null, function (json) {
                                 if (json.d == 1) {
                                     alertbox("Success fully closed");
                                     Getticketdetails($("#serviceid").val());
                                     var ss = "<span style='cursor:pointer;color:blue;' onclick='javascript: Getticketdetails(\"" + $("#serviceid").val() + "\")'>View</span>";
                                     var tbl = $('#example').dataTable();
                                     var oCell = tbl[0].rows[aPos + 1].cells[8]
                                     oCell.innerHTML = "";
                                     oCell.innerHTML = ss.toString();
                                     oTable.fnUpdate("Closed", aPos, 7);
                                 }
                             });

                         }

                     }
                 });
             }

      }

      function reopenservice() {

          confirmbox("Are you sure you want to Reopen the Service?");
          $("#dialog-confirm").dialog({
              close: function (event, ui) {
                  if (confirmresult) {
                      $.getJSON('ServiceingJsonT1.aspx?serviceid=' + $("#serviceid").val() + '&u=' + $("#hidloginuser").val() + '&opr=9&r=' + Math.random(), null, function (json) {
                          if (json.d == 1) {
                              alertbox("Success fully Opened");
                              Getticketdetails($("#serviceid").val());
                              var ss = "<span style='cursor:pointer;color:blue;' onclick='javascript: Getticketdetails(\"" + $("#serviceid").val() + "\")'>View</span>";

                              var tbl = $('#example').dataTable();
                              var oCell = tbl[0].rows[aPos + 1].cells[8]
                              oCell.innerHTML = "";
                              oCell.innerHTML = ss.toString();
                              oTable.fnUpdate("Reopend", aPos, 7);
                          }
                      });

                  }

              }
          });

      }


      function changeservicedate() {
          var dt = $("#txtserdate").val().trim();
          if (dt == "") {
             alertbox("Select Date Of Service  ");
         }
         else {
             confirmbox("Are you sure you want to Change the Service Date?");
             $("#dialog-confirm").dialog({
                 close: function (event, ui) {
                     if (confirmresult) {
                         $.getJSON('ServiceingJsonT1.aspx?serviceid=' + $("#serviceid").val() + '&srvdt=' + $("#txtserdate").val() + '&u=' + $("#hidloginuser").val() + '&opr=8&r=' + Math.random(), null, function (json) {
                             if (json.d == 1) {
                                 alertbox("Successfully Changed..");
                                 Getticketdetails($("#serviceid").val());
                                 var ss = "<span style='cursor:pointer;color:blue;' onclick='javascript: Getticketdetails(\"" + $("#serviceid").val() + "\")'>View</span>";

                                 var tbl = $('#example').dataTable();
                                 var oCell = tbl[0].rows[aPos + 1].cells[8]
                                 oCell.innerHTML = "";
                                 oCell.innerHTML = ss.toString();
                                 oTable.fnUpdate("Service Date Changed To " + $("#txtserdate").val(), aPos, 7);
                                 $("#div-date").dialog("close");
                             }
                         });

                     }

                 }
             });
         }

      }
      function Receiveservice() {

          confirmbox("Are you sure you want to Receive the Service?");
          $("#dialog-confirm").dialog({
              close: function (event, ui) {
                  if (confirmresult) {
                      $.getJSON('ServiceingJsonT1.aspx?serviceid=' + $("#serviceid").val() + '&u=' + $("#hidloginuser").val() + '&opr=7&r=' + Math.random(), null, function (json) {
                          if (json.d == 1) {
                              alertbox("Success fully Received");
                              Getticketdetails($("#serviceid").val());
                              var ss = "<span style='cursor:pointer;color:blue;' onclick='javascript: Getticketdetails(\"" + $("#serviceid").val() + "\")'>View</span>";

                              var tbl = $('#example').dataTable();
                              var oCell = tbl[0].rows[aPos + 1].cells[8]
                              oCell.innerHTML = "";
                              oCell.innerHTML = ss.toString();
                              oTable.fnUpdate("Request Received", aPos, 7);
                          }
                      });

                  }

              }
          });

      }

      function OpenServicePopup() {
          $("#div-date").dialog("open");
      }


</script>
<style type="text/css" >
         
        ul, ol {
        padding: 0;
        margin: 0 0 0px 0px;
        }
        .comment {
        padding-right: 65px;
        }
        
        .comment {

        margin: 0 0 3px;
        padding: 3px 15px 8px;
        background-color: #ffff;
        position: relative;
        word-wrap: break-word;
        overflow-wrap: break-word;
        border-radius: 10px;
          border :#030303;
        }
        .comment-meta {
        position: absolute;
        right: 15px;
        top: 1px;
        }.commentmetadata {
        font-size: 10px;
        }
        .comment p {
        line-height: 10px;
        clear: both;
        font-size: 11px;
        margin-bottom: 0.5em;
        }
    .hor-minimalist-b
        {
            font-family: "Verdana";
            font-size: 11px;
            background: #fff;
            margin: 0px;
            width: 520px;
            border-collapse: collapse;
            text-align: left;
        }
        .hor-minimalist-b th
        {
            font-size: 14px;
            font-weight: normal;
            color: #039;
            padding: 3px 2px;
            border-bottom: 1px solid #6678b1;
        }
        .hor-minimalist-b td
        {
            border-bottom: 1px solid #ccc;
            color: #669;
            padding: 4px 6px;
        }
        .hor-minimalist-b tbody tr:hover td
        {
            color: #009;
        }
        button {
	cursor: pointer;

}
a.button:active
{
	border-color: #4B8DF8;
}
 a.button:hover
 {
 	color:White;
	
	border: 1px solid #2F5BB7 !important;
	
	background: #3F83F1;
	background: -webkit-linear-gradient(top, #4D90FE, #357AE8);
	background: -moz-linear-gradient(top, #4D90FE, #357AE8);
	background: -ms-linear-gradient(top, #4D90FE, #357AE8);
	background: -o-linear-gradient(top, #4D90FE, #357AE8);
	
 }
a.button
{
	text-align:center;
	font: bold 11px Helvetica, Arial, sans-serif;
	cursor: pointer;
	
	
	display: inline-block;
	width:74px;
	border: 1px solid #3079ED !important;
	color:White;
	height:14px;

	background: #4B8DF8;
	background: -webkit-linear-gradient(top, #4C8FFD, #4787ED);
	background: -moz-linear-gradient(top, #4C8FFD, #4787ED);
	background: -ms-linear-gradient(top, #4C8FFD, #4787ED);
	background: #4B8DF8;
	
	-webkit-transition: border .20s;
	-moz-transition: border .20s;
	-o-transition: border .20s;
	transition: border .20s;
	margin: 5px;
	padding: 3px 5px 5px 3px;
}
 .textbox1
        {
        height :20px;
        width: 180px;
        border-right: #cbd6e4 1px solid;
        border-top: #cbd6e4 1px solid;
        border-left: #cbd6e4 1px solid;
        color: #0b3d62;
        border-bottom: #cbd6e4 1px solid;
        }
        .dropdown1
        {
        height :25px;
        width: 182px;
        border-right: #cbd6e4 1px solid;
        border-top: #cbd6e4 1px solid;
        border-left: #cbd6e4 1px solid;
        color: #0b3d62;
        border-bottom: #cbd6e4 1px solid;
        }
        
        .badge {
            background: radial-gradient( 5px -9px, circle, white 8%, red 26px );
            background-color: red;
            border: 2px solid white;
            border-radius: 12px;
            box-shadow: 1px 1px 1px black;
            color: white;
            font: bold 10px/7px Helvetica, Verdana, Tahoma;
            height: 10px;
            min-width: 8px;
            padding: 4px 3px 0 3px;
            text-align: center;
            }
        .badge {
            float: left;
            left: 60px;
            margin: -10px;
            position: relative;
            top: 5px; 
        }
        
          tr.even.selected
        {
            background-color: #00cccc;
        }
        tr.odd.selected
        {
            background-color: #00cccc;
        }
        
        tr.even.selected td.sorting_1
        {
            background-color: #00cccc;
        }
        tr.odd.selected td.sorting_1
        {
            background-color: #00cccc;
        }

</style>
</head>
<body>
    <form id="form1" runat="server">
    <center>
     <div style="width: 1300px; margin: auto;">
       <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Service Management</b>
     <br />
       <table >
     
       <tr><td colspan ="2">
         <table width="1300" cellpadding="0" cellspacing="0" border="0" align="center" class="display" id="example" style="font-size: 11px; font-family: verdana">
          <thead style="text-align: left; font-weight: bold;">
            <tr>
              <th>SNo.</th>
              <th >User Name</th>
              <th >Service Id</th>
              <th >Raised Date</th>
              <th >Service Req Date</th>
              <th >Plateno</th>
               <th >Group</th>
              <th >Service Type</th>
              <th >Priority</th>
              <th >Status</th>
             <%-- <th >Assign By</th>--%>
              <th >Assign</th>
            </tr>
          </thead>
          <tbody></tbody>
          <tfoot style="text-align: left; ">
            <tr>
              <th>SNo.</th>
              <th >User Name</th>
              <th >Service Id</th>
              <th >Raised Date</th>
               <th >Service Req Date</th>
               
              <th >Plateno</th>
               <th >Group</th>
              <th >Service Type</th>
              <th >Priority</th>
              <th >Status</th>
             <%-- <th >Assign By</th>--%>
              <th >Assign</th>
            </tr>
          </tfoot>
        </table>
       </td>
       </tr>
       </table>
      
     <input type ="hidden" id="hidrole" runat ="server" />
     <input type ="hidden" id="hidloginuser" runat ="server" />
     <input type ="hidden" id="serviceid" runat ="server" />
    </div>
    </center>
    <div id="dialog-message"  style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 11px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;">
        <br />
        <div class="table info_box" style="width:500px;">
        <table width="width:500px;" style="padding-left:10px;font-size: 11px;font-family:verdana;font-weight: bold;color:#4E6CA3;">
        <tr align="left" ><td>Assigned To</td><td>:</td>
        <td >
        <input type ="text" class ="textbox1 " id="txtaasign" />
        </td>
        </tr>
        <tr align="left" ><td colspan="3">Remarks</td></tr>
        <tr align="left" ><td colspan="3">
          <textarea style="width :480px;height:100px;" id="txtdescri" rows="15" cols="50" ></textarea>
        </td>
        </tr>
        </table>
        </div>
    </div>
     <div id="dialog-close"  style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 11px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;">
        <br />
        <div class="table info_box" style="width:500px;">
        <table width="width:500px;" style="padding-left:10px;font-size: 11px;font-family:verdana;font-weight: bold;color:#4E6CA3;">
        <tr align="left" ><td>Service Compleated Date</td><td>:</td>
        <td >
        <input type ="text" class ="textbox1 " id="txtservicecompdate" />
        </td>
        </tr>
        <tr align="left" ><td colspan="3">Remarks</td></tr>
        <tr align="left" ><td colspan="3">
          <textarea style="width :480px;height:100px;" id="txtservicecompremarks" rows="15" cols="50" ></textarea>
        </td>
        </tr>
        </table>
        </div>
    </div>
    <div id="div-date"  style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 11px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;">
        <br />
        <div class="table info_box" style="width:250px;">
        <table width="width:220px;" style="padding-left:10px;font-size: 11px;font-family:verdana;font-weight: bold;color:#4E6CA3;">
        <tr align="left" ><td>Changed To</td><td>:</td>
        <td >
        <input type ="text" class ="textbox1 " id="txtserdate" style="width :144px;" />
        </td>
        </tr>
       
        </table>
        </div>
    </div>
     <div id="div-ticket"  style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 11px; padding-left: 0px; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;">
        <br />
        <table width="500px" style="padding-left:10px;font-size: 12px; font-family: verdana;" >
        <tbody >
        <tr><td colspan ="2" align ="left" ><a class ="button" id="btnreq" onclick="Receiveservice()" style ="width:110px;" >Receive Service</a></td><td colspan ="2" align ="left" ><a class ="button" id="btnsrdt" onclick="OpenServicePopup()" style ="width:140px;" >Change Service Date</a></td><td colspan ="2" align ="right" style ="padding-right: 25px;" ><a class ="button" id="btnclose" onclick="openCloseDialog()" style ="width:110px;" >Close Service</a><a class ="button" id="btnreopen" onclick="reopenservice()" style ="width:110px;" >Reopen Service</a></td></tr>
        <tr><td colspan="6"  >
        <table  class="hor-minimalist-b" >
        <tr><td style ="width :120px;">Plateno</td><td   style ="width :10px;">:</td><td id="plateno"></td><td  style ="width :80px;">Status</td><td  style ="width :10px;">:</td><td id="status"></td></tr>
        <tr><td  style ="width :120px;">Priority</td><td  style ="width :10px;">:</td><td id="priority"></td><td  style ="width :80px;" >Assigned To</td><td  style ="width :10px;">:</td><td id="assigned"></td></tr>
        <tr><td  style ="width :120px;">Assigned Date Time</td>
        <td  style ="width :10px;">:</td><td id="datetime" colspan="4"></td></tr>
        <tr><td  style ="width :120px;">Closed Date Time</td><td  style ="width :10px;">:</td><td id="tdcloseddt" colspan="4"></td></tr>
        <tr><td ><u>Remarks</u></td><td colspan ="5"></td></tr>
        <tr>
        <td colspan ="6" id="tdDescription" >
        </td>
        </tr>
        </table>
        </td>
        </tr>
        <tr><td colspan="6"  >Comments</td></tr>
        <tr><td colspan ="6"  >
        <div >
         <ul id="ullist">
       
        </ul>
        </div>
        </td></tr>
        <tr><td colspan ="6" >Comment:</td></tr>
        <tr>
        <td colspan ="6"  >
           <textarea style="width :510px;height:100px;" id="txtcomment" rows="15" cols="50" ></textarea>
         </td>
         </tr>
           <tr><td align ="right" colspan ="6" style ="text-align:right;" >
         <button id="btnsend" type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only ui-state-hover" onclick="javascript:sendcomment()" role="button" aria-disabled="false">
          <span  style="cursor:pointer;" class="ui-button-text"  >send</span></button>
         </td></tr>
        </tbody>
        </table>
       
    </div>
    <div class="demo">
        <div id="dialog-confirm" title="Confirmation ">
            <p id="displayc">
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            </p>
        </div>
        <div id="dialog-alert" title="Information">
            <p id="displayp">
                <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
                </span>
            </p>
        </div>
        </div>
    </form>
</body>
</html>

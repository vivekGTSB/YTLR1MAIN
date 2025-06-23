<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.WebAlerts" Codebehind="WebAlerts.aspx.vb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8">
    <link rel="shortcut icon" type="image/ico" href="images/alerticon.ico">
    <title>Web Alerts</title>
    <style type="text/css" title="currentStyle">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <style type="text/css">
        #fw_container
        {
            width: 100%;
        }
        table.display td
        {
            padding: 2px 2px;
        }
    </style>
   <%-- <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.0/themes/redmond/jquery-ui.css" />--%>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <script src="jsfiles/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="jsfiles/jquery-ui.min.js" type="text/javascript"></script>
    <%--<script src="http://code.jquery.com/jquery-1.8.3.js"></script>
    <script src="http://code.jquery.com/ui/1.10.0/jquery-ui.js"></script>--%>
   
    <style>
        fieldset
        {
            padding: 0;
            border: 0;
            margin-top: 25px;
        }
        .ui-dialog .ui-dialog-titlebar-close span
        {
            display: block;
            margin: 0px;
        }
        #dt_example ul
        {
            list-style-type: none;
        }
        #txtRemarks
        {
            width: 276px;
            height: 52px;
        }
        
        
    </style>
    <script src="jsfiles/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="jsfiles/jquery.jeditable.js" type="text/javascript"></script>
    <script src="jsfiles/jquery.validate.js" type="text/javascript"></script>
    <script src="jsfiles/jquery.dataTables.editable.js" type="text/javascript"></script>
    <script type="text/javascript" charset="utf-8">
		    var oTable;
		    $(document).ready(function () {
            $("#dialog:ui-dialog").dialog("destroy");
            var resp;
    $("#dialog-message").dialog({
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
            $( "#dialog-form" ).dialog({
      autoOpen: false,
      height: 450,
      width: 350,
      minHeight: 400,
      draggable:false,
      resizable: false,
      minWidth: 300,
      modal: true,
      buttons: {
        "Resolve": function() {
        var remarksData= $("#txtRemarks").val();
        if ($.trim(remarksData) == "") {
       alertbox("Please enter remarks..!!");
          }
          else{

            var reason=$('input[name=rdbReason]:checked', '#resolve_form').val();
            var remarks =$("#txtRemarks").val();
            $.get("UpdateWebData.aspx?id="+ $("#resolve_id").val() +"&reason="+reason +"&remarks="+ remarks  ,function (data) {
                 refreshTable();
            });
             
            $( this ).dialog( "close" );  
            }       
        },
        Cancel: function() {
          $( this ).dialog( "close" );
        }
      },
      close: function() {
        $( this ).dialog( "close" );
      }
    });


    var alerttp="";



		      oTable=  $('#example').dataTable({
		            "bProcessing": false,
		            "bJQueryUI": true,
		            "sAjaxSource": "GetWebAlerts.aspx",
                    "sPaginationType": "full_numbers",
                    "aLengthMenu": [ 10,25,50,100,200,500 ],
                    "iDisplayLength" : 25,
                     "fnDrawCallback": function (oSettings) {
                       if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[1].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                    "aaSorting": [[3, "desc"]],
		            aoColumns: [
                     { "bVisible": false, "bSortable": false, "aTargets": [0] },
                   {"sClass" : "left", "bVisible": true, "bSortable": false, "aTargets": [1] },
                     { "bVisible": true, "bSortable": true, "aTargets": [2] },
                     { "bVisible": true, "bSortable": true, "aTargets": [3] },
                     { "bVisible": true, "bSortable": true,
                        "fnRender": function (oData, sVal) {
                        alerttp=sVal;
                            if (sVal.indexOf("OVERSPEED") >= 0) {                            
                           return "<span title='Overspeed = " +  oData.aData[7] + " KM/H'>" + sVal + "(" + oData.aData[7] +" KM/H)</span>";                                                        
                           }
                            else if (sVal.indexOf("IDLING") >= 0) {
                                return "<span title='Idling from = " + oData.aData[7].split(";")[0] + " For "+ oData.aData[7].split(";")[1] + " Mins'>" + sVal + "("+ oData.aData[7].split(";")[1]+  " Mins)</span>";
                                }
                             else if(sVal.indexOf("Geofence In") >= 0)
                             {
                                return "<span>" + oData.aData[7] + " - IN</span>";
                             }
                              else if(sVal.indexOf("Geofence out") >= 0)
                             {
                                return "<span>" + oData.aData[7] + " - OUT</span>";
                             }
                            else { 

                            return sVal;                              
                             }
                        }, "aTargets": [4], "asSorting": ["desc", "asc"] },
                        
                   { "bVisible": true, "bSortable": true, "aTargets": [5] },
                     { "bVisible": true, "bSortable": true, "aTargets": [6] },
                    { "bVisible": true, "bSortable": true,
                        "fnRender": function (oData, sVal) {

                                           return oData.aData[8];                              
                            
                        }, "aTargets": [8], "asSorting": ["desc", "asc"] },

                        { "bVisible": true, "bSortable": true,
                        "fnRender": function (oData, sVal) {

                                      return oData.aData[9];                              
                           
                        }, "aTargets": [9], "asSorting": ["desc", "asc"] },
                         { "bVisible": true, "bSortable": true,
                        "fnRender": function (oData, sVal) {
                        if (oData.aData[10] =="Yes") {
                             return "<span onclick='javascript:alertbox(\" Already Resolved.. \");' style='text-decoration:underline; color:Grey;cursor:pointer;'>Resolved</span>";
                        }
                        else{
                        var id=oData.aData[0];
                        var plateno=oData.aData[2];
                        var datetime=oData.aData[3];
                        var alerttype=alerttp;
                        return "<span onclick=\"openResolve('"+ id +"','"+ plateno +"','"+ datetime +"','"+ alerttype +"')\" style=\"text-decoration:underline; color:Blue;cursor:pointer;\">Resolve It</span>";
                        }   
                        }, "aTargets": [10], "asSorting": ["desc", "asc"] },

                      
                   
                     
                         ]
                         
                     }).makeEditable({
                                        sUpdateURL: "UpdateWebData.aspx?i=0",
                                        "aoColumns": [
                                             null,
                                               null,
                                               null,
                                               null,
                                                null,
                                               null,
                                                null,
                                                 null,
                                               null
                                               
                                         ],
                                           sDeleteURL: "UpdateWebData.aspx?i=1",
                                           oDeleteRowButtonOptions: {	label: "Remove", 
													icons: {primary:'ui-icon-trash'}
									    },

                                    });
                               });

                                function refreshTable() {
                                    table = oTable.dataTable();
                                    oSettings = table.fnSettings();
                                    $.getJSON('GetWebAlerts.aspx', null, function (json) {
                                        table.fnClearTable(this);

                                        for (var i = 0; i < json.aaData.length; i++) {
                                            table.oApi._fnAddData(oSettings, json.aaData[i]);
                                        }

                                        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                                        table.fnDraw();
                                    });
                                    return false;
                                }
    </script>
    <script type="text/javascript">
        function alertbox(message) {
            document.getElementById("displayp").innerHTML = message;
            $("#dialog-message").dialog("open");
        }
        function hideall() {
            $("#btnDeleteRow").hide();
        }
        function openResolve(id, plateno, datetime, alerttype) {
            $("#resolve_id").val(id);
            $("#resolve_event").text(alerttype);
            $("#resolve_plate_no").text(plateno);
            $("#resolve_timestamp").text(datetime);
            $("#txtRemarks").val("");
            $("#resolve_form").find('input:radio, input:checkbox')
         .removeAttr('checked').removeAttr('selected');
            if (alerttype.indexOf("Geofence") >= 0) {
            $("#r7").attr('checked', 'checked');
            }
            else{
            $("#r1").attr('checked', 'checked');
        }
            $("#txtRemarks").focus();
            $("#dialog-form").dialog("open");
        }

    </script>
</head>
<body id="dt_example" onload="hideall()" style="font-size: 11px; font-weight: bold;
    font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <center>
        <br />
        <br />
        <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Web Alerts</b>
    </center>
    <div id="fw_container">
        <table width="100%" cellpadding="0" cellspacing="0" border="0" class="display" id="example"
            style="font-size: 10px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <thead>
                <tr>
                    <th>
                        ID
                    </th>
                    <th style="width: 15px">
                        S No
                    </th>
                    <th style="width: 90px">
                        Plate No
                    </th>
                    <th style="width: 130px">
                        DateTime
                    </th>
                    <th style="width: 200px;">
                        Alert Type
                    </th>
                    <th style="width: 150px">
                        Event Reason
                    </th>
                    <th style="width: 245px">
                        Remark
                    </th>
                    <th style="width: 90px">
                        Resolved By
                    </th>
                    <th style="width: 130px">
                        Resolved Date
                    </th>
                    <th style="width: 35px">
                        Status
                    </th>
                </tr>
            </thead>
            <tfoot>
                <tr>
                    <th>
                        ID
                    </th>
                    <th>
                        S No
                    </th>
                    <th>
                        Plate No
                    </th>
                    <th>
                        DateTime
                    </th>
                    <th>
                        Alert Type
                    </th>
                    <th>
                        Event Reason
                    </th>
                    <th>
                        Remark
                    </th>
                    <th>
                        Resolved By
                    </th>
                    <th>
                        Resolved Date
                    </th>
                    <th>
                        Status
                    </th>
                </tr>
            </tfoot>
            <tbody>
            </tbody>
        </table>
    </div>
    <div id="dialog-form" title="Resolve Alert">
        <div>
            <form id="resolve_form">
            <input type="hidden" id="resolve_id" />
            <h3>
                <span id="resolve_event"></span>- <small><span id="resolve_plate_no"></span></small>
            </h3>
            <div>
                <small><span id="resolve_timestamp" class="easydate"></span></small>
            </div>
            <div style="clear: both">
            </div>
            <br />
            <div>
                <label>
                    Reason:</label>
                <ul>
                    <li>
                        <input id="r1" type="radio" name="rdbReason" value="6"  />
                        Other</li>
                    <li>
                        <input id="r2" type="radio" name="rdbReason" value="3" />
                        In Workshop </li>
                    <li>
                        <input id="r3" type="radio" name="rdbReason" value="2" />
                        Battery Taken Out</li>
                    <li>
                        <input id="r4" type="radio" name="rdbReason" value="5" />
                        Signal Lost</li>
                    <li>
                        <input id="r5" type="radio" name="rdbReason" value="4" />
                        Not In Operation</li>
                    <li>
                        <input id="r6" type="radio" name="rdbReason" value="1" />
                        Accident</li>
                    <li>
                        <input id="r7" type="radio" name="rdbReason" value="7" />
                        Geofence</li>
                </ul>
            </div>
            <div>
                <label>
                    Remarks:</label><br />
                <ul>
                    <li>
                        <textarea id="txtRemarks"> </textarea></li>
                </ul>
            </div>
            </form>
        </div>
    </div>
    <div class="demo">
        <div id="dialog-message" title="Alert">
            <p id="displayp">
                <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
                </span>
            </p>
        </div>
    </div>
</body>
</html>

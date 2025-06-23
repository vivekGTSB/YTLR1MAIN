<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.LiveTrack" CodeBehind="LiveTrack.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Live Track</title>
    <style type="text/css">
        .dynamicDiv {
            font-size: 11px;
            float: left;
        }

        .dynamictext {
            font-size: 14px;
            float: left;
        }

        .dynamicDivH {
            border: solid 1px Black;
            font-size: 11px;
            padding: 3px;
            float: left;
        }
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link type="text/css" href="GMap/chosen/chosen.css" rel="stylesheet" />
    <link type="text/css" href="cssfiles/css3-buttons.css" rel="stylesheet" />
    <link type="text/css" href="cssfiles/tooltip.css" rel="stylesheet" />
    <link type="text/css" href="GMap/VehicleMarker.css" rel="stylesheet" />
    <style type="text/css">
        .group-option {
            text-align: left;
        }

        .chzn-single {
            text-align: left;
        }

        .group-result {
            text-align: left;
        }

        div.demo {
            padding: 0px;
        }
    </style>
    <!--[if lt IE 9]>
    <style type="text/css">
    .chat-bubble-arrow-border {
    bottom:-36px;
    }
    .chat-bubble-arrow {
    bottom:-34px;
    }   
    .b-arrow{
    bottom:-30px;
    }
     .r-arrow-border {
    bottom:-31px;
    }
    .r-arrow{
    bottom:-30px;
    }
    .g-arrow{
   bottom:-30px;
    }
    </style>    
<![endif]-->
    <style type="text/css">
        .chzn-results {
            width: 440px;
        }

        .chzn-container {
            width: 450px;
        }

        .chzn-search {
            width: 370px;
        }
    </style>
    <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
    <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
    <script src="GMap/chosen/chosen.jquery.js" type="text/javascript"></script>
    <script src="jsfiles/tooltip.js" type="text/javascript"></script>
    <%--<script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?v=3&sensor=false&libraries=drawing"></script>--%>
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?v=3.28&client=gme-zigbeeautomation&sensor=false&libraries=drawing&channel=YTL"></script>
    <script src="GMap/richmarker.js" type="text/javascript"></script>
    <script src="jsfiles/GenerateMap.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {

            if ($.browser.msie && ($.browser.version === "8.0" || $.browser.version === "7.0" || $.browser.version === "6.0")) {

            }
            else {

                $(".chzn-select").chosen();

                $(".chzn-select").chosen({ no_results_text: "Translated No results matched" });
                $(".chzn-select").chosen({ allow_multi_select: true });
                $('.chzn-search').find('input[type="text"]')[0].style.width = '405px'
                //$(".chzn-select").chosen();
                //$(".chzn-select-deselect").chosen({ allow_single_deselect: false });
                //$('.chzn-search').find('input[type="text"]')[0].style.width = '405px'
            }
            $("#dialog:ui-dialog").dialog("destroy");

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
        });
    </script>
    <script type="text/javascript">
        function setDDL() {
            $(".chzn-drop").width(448);
            $("#ddlplate_chzn").width(450);
        }

        //function createDivAll() {
        //    if ($("#ddlplate").val().length > 15) {
        //        alert("The count of vehicles limited to 15 at a time. Please deselect and submit.");
        //        return false;
        //    }
        //    else {
        //        for (var i = 0; i < $("#ddlplate").val().length; i++) {
        //            createDiv($("#ddlplate").val()[i]);
        //        }
        //    }

        //}

    </script>
</head>
<body onload="javascript:setDDL()">
    <form id="form1" runat="server">
        <center>
            <table border="0" cellpadding="0" cellspacing="0" style="padding-top: 5px;">
                <tr>
                    <td>
                        <b style="font-family: Verdana; font-size: 12px; color: #38678B;">Select Plate No:</b>
                        <%= sb.ToString() %>
                    </td>
                    <td>
                        <input type="button" id="ImageButton1" title="Track" class="action blue" onclick="javascript: return createDiv();"
                            value="Track" />
                    </td>
                </tr>
            </table>
            <hr style="color: Blue; margin-bottom: 5px; margin-top: 1px;" />
        </center>
    </form>
    <div class="demo">
        <div id="dialog-message" title="Live Tracking Alert">
            <p id="displayp">
                <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;"></span>
            </p>
        </div>
    </div>
</body>
</html>

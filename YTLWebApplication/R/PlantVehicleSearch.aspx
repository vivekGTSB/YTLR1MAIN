<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.PlantVehicleSearch" CodeBehind="PlantVehicleSearch.aspx.vb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Truck Info</title>
    <%--<link href="css/bootstrap.min.css" rel="stylesheet" />--%>
    <link href="GMap/default.css" rel="stylesheet" type="text/css" />
    <link href="GMap/VehicleMarker.css" rel="stylesheet" />

    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
    </style>
    <link href="font-awesome/css/font-awesome.css" rel="stylesheet" />
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <link href="css/gdropdown2.css" rel="stylesheet" type="text/css" />
    <script src="js/gdropdown2.js" type="text/javascript"></script>
    <%--  <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>--%>

    <style>
        .row {
            margin-right: 0px;
            margin-left: 0px;
        }

        .hideshowdiv {
            position: absolute;
            top: 0px;
            right: 600px;
            visibility: visible;
        }

        .hideshowdiv2 {
            position: absolute;
            top: 80px;
            left: 0px;
            visibility: visible;
        }

        .align {
            margin-top: 5px;
            margin-bottom: 5px;
        }

        .container.no-print {
            left: 150px !important;
        }

        .VehicleMarker_OVERLAY {
            border-width: 0px;
            border: none;
            position: absolute;
            padding: 0px 0px 0px 0px;
            margin: 0px 0px 0px 0px;
            z-index: 99 !important;
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

    <style type="text/css">
        #over_map {
            z-index: 99;
            background: #EEE;
            -webkit-border-radius: 10px;
            border-radius: 10px;
            display: block;
            border: 1px solid #AAA;
            border-right: 1px solid #AAA;
            box-shadow: 2px 2px 3px #888;
            border-bottom: 1px solid #888;
            border-right: 1px solid #888;
            border-left: 1px solid #888;
        }

        #over_map2 {
            z-index: 99;
            background: #EEE;
            -webkit-border-radius: 10px;
            border-radius: 10px;
            display: block;
            border: 1px solid #AAA;
            border-right: 1px solid #AAA;
            box-shadow: 2px 2px 3px #888;
            border-bottom: 1px solid #888;
            border-right: 1px solid #888;
            border-left: 1px solid #888;
        }

        #uha {
            margin: 0 auto;
            padding: 0;
            width: 350px;
            z-index: 99999;
        }

        #uha2 {
            margin: 0 auto;
            padding: 0;
            width: 700px;
            z-index: 99999;
        }

        a:focus {
            outline: none;
        }

        #panel {
            display: none;
            font: 12px Arial, Helvetica, sans-serif;
        }

        .btn-slide {
            text-align: center;
            width: 50px;
            height: 20px;
            padding: 0px 10px 0 0;
            margin: 0px 130px;
            display: block;
            font: bold 120%/100% Arial, Helvetica, sans-serif;
            color: #fff;
            text-decoration: none;
        }

        .active {
            background-position: right 12px;
        }

        .vlabel {
            background: #FFFF00;
            padding: 2px;
            border: solid 1px black;
            font-family: Verdana;
            font-size: 11px;
        }

        #examples_wrapper {
            font-size: 12px;
        }

        .dataTables_filter {
            width: 80%;
            float: right;
            text-align: right;
            margin-top: -20px;
        }

        .fontfix {
            font-size: 10px;
            white-space: nowrap;
        }
    </style>

    <style>
        .jaffa {
            float: left;
            position: absolute;
            top: 100px;
            left: -110;
            display: none;
            background: #fff;
            -moz-border-radius-topright: 20px;
            -webkit-border-top-right-radius: 20px;
            -moz-border-radius-bottomright: 20px;
            -webkit-border-bottom-right-radius: 20px;
            width: 330px;
            height: auto;
            padding: 15px 15px 15px 110px;
            filter: alpha(opacity=85);
            opacity: .95;
            border: black 1px solid;
        }

        a.trigger {
            font-weight: bold;
            position: absolute;
            text-decoration: none;
            left: 0;
            font-size: 13px;
            letter-spacing: -1px;
            font-family: Arial, sans-serif;
            color: #fff;
            padding: 10px 34px 10px 5px;
            background: #fff url(images/plus.png) 85% 55% no-repeat;
            display: block;
            color: Black;
        }

            a.trigger:hover {
                font-weight: bold;
                position: absolute;
                text-decoration: none;
                left: 0;
                font-size: 13px;
                letter-spacing: -1px;
                font-family: Arial, sans-serif;
                color: #fff;
                padding: 10px 34px 10px 5px;
                background: #fff url(images/plus.png) 85% 55% no-repeat;
                display: block;
                color: Black;
            }

        a.active.trigger {
            background: #fff url(images/minus.png) 85% 55% no-repeat;
            color: Black;
            font-weight: bold;
        }
    </style>
</head>
<body onresize="resize()">
    <form id="form1" runat="server">
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
        <input type="text" style="display: none" id="hdnselect" />
        <div id="uha" class="hideshowdiv">
            <div id="panel">
                <div class="row container" id="over_map" style="width: 100%; margin: 0px !important">
                    <table style="width: 350px;">
                        <tr>
                            <td align="left">
                                <b style="color: #465AE8;">Plant </b>
                            </td>
                            <td>
                                <b style="color: #465AE8;">:</b>
                            </td>
                            <td align="left" style="width: 326px">
                                <select id="ddlplant" class="form-control" name="ddlplant" runat="server"></select>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <b style="color: #465AE8;">Distance </b>
                            </td>
                            <td>
                                <b style="color: #465AE8;">:</b>
                            </td>
                            <td align="left" style="width: 326px">
                                <select id="ddldistance" class="form-control" name="ddldistance" runat="server">
                                    <option value="50">50 Kms Away</option>
                                   <option value="100">100 Kms Away</option>
                                    <option value="200">200 Kms Away</option>
                                    <option value="300">300 Kms Away</option>
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <b style="color: #465AE8;">Transporter </b>
                            </td>
                            <td>
                                <b style="color: #465AE8;">:</b>
                            </td>
                            <td align="left" style="width: 326px">
                                <select id="ddltransporter" class="form-control" name="ddltransporter" runat="server">
                                    <option value="ALL">ALL</option>
                                    <option value="1">Internal</option>
                                    <option value="2">External</option>
                                </select>
                            </td>
                        </tr>
                         <tr>
                            <td align="left">
                                <b style="color: #465AE8;">Type </b>
                            </td>
                            <td>
                                <b style="color: #465AE8;">:</b>
                            </td>
                            <td align="left" style="width: 326px">
                               <select name="txtType" id="ddlType" class ="form-control"  runat ="server" >
	<option value="0">--Select Vehicle Type--</option>
	<option value="CARGO">CARGO</option>
	<option value="PM">PM</option>
	<option value="TANKER">TANKER</option>
	<option value="TIPPERS">TIPPERS</option>
</select>
                            </td>
                        </tr>
                        
                        <tr>

                            <td colspan="3" align="center">
                                <br />
                                <a href="javascript:SearchTrucks();" class="button" style="width: 80px"><span class="ui-button-text"
                                    title="Search Trucks">Search Trucks</span> </a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>

            <a href="#" class="btn-slide slide">
                <img id="udimg" src="images/down.gif" alt="Open vehicle search" title="Open vehicle search"
                    style="visibility: visible; border: 0px none FFFFFF" /></a>
        </div>

        <div id="uha2" class="hideshowdiv2">
            <%--<a href="#" class="btn-slide2 slide">
            <img id="udimg2" src="images/down.gif" alt="Open vehicle search" title="Open vehicle search"
                style="visibility: visible; border: 0px none FFFFFF" /></a>--%>

            <a class="trigger" href="#" style="z-index: 999; margin-top: -50px; border-bottom-right-radius: 25px; border-top-right-radius: 25px;"
                title="Close vehicle search">
                <img src="images/fleeticon.png" style="height: 30px; width: 30px;" alt="Vehicle"></a>

            <div id="panel2">
                <div class="row container" id="over_map2" style="width: 100%">
                    <div class="row align">
                        <div class="col-md-12">
                            <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
                                <thead>
                                    <tr>
                                        <th>SNo</th>
                                        <th style="width: 130px">Plateno</th>
                                        <th>Transporter Name</th>
                                        <th>Transporter Type</th>
                                        <th>Distance (KM)</th>
                                        <th>Curr. Location</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div id="map"></div>
        </div>
    </form>

    <script src="js/jquery.js"></script>
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?v=3.28&client=gme-zigbeeautomation&libraries=drawing&channel=YTL"></script>
    <script src="GMap/richmarker.js" type="text/javascript"></script>
    <script src="GMap/markerclusterer.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript">
        function getWindowWidth() { if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement && document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth; }
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }
        function resize() {
            var map = document.getElementById("map");
            map.style.height = getWindowHeight() + "px";
            map.style.width = getWindowWidth() + "px";
            document.getElementById("uha").style.right = parseInt(map.style.width) / 3;
            //document.getElementById("search1").style.right = "8px";
        }
    </script>
    <script>
        var arrowpath = "M15 0 L1 30 L31 30 Z M 1  30 q 15 -12 30 0";
        var oTable;
        var map;
        var markerClusterer;
        var vehiclemarkercustomMapCanvas;
        var zoommap = 10;
        var lonmap = 101.7000;
        var latmap = 3.1597;
        var Platenobool = false;
        var JobDescbool = false;
        var clusterbool = false;
        var jobmarkerCluster = null;
        var dragging = false;
        var mapsize = { width: window.innerWidth, height: window.innerHeight };
        var gridSizes = [100, 100, 100, 100, 100, 20, 30, 30, 40, 40, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100];
        mcp = new google.maps.LatLng(latmap, lonmap);
        var customMapCanvas;
        function callme() {
            document.getElementById("platenotext").click();
            document.getElementById("jobtext").click();
            document.getElementById("mcluster").click();
            //   DrawImage();
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

        $(function () {
            resize();
            $("#panel").show();

            var mapOptions = {
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                scrollwheel: true,
                overviewMapControl: true,
                streetViewControl: false,
                panControl: true,
                zoomControl: true,

                mapTypeControl: true,
                gestureHandling: 'greedy',
                maxZoom: 20,
                mapTypeControlOptions: {
                    style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
                    position: google.maps.ControlPosition.TOP_RIGHT
                },
                zoomControlOptions: {
                    position: google.maps.ControlPosition.RIGHT_BOTTOM
                },
                panControlOptions: {
                    position: google.maps.ControlPosition.LEFT_TOP
                }

            };

            map = new google.maps.Map(document.getElementById('map'), mapOptions);
            map.setCenter(mcp);
            map.setZoom(parseInt(zoommap));

            var checkOptions8 = {
                gmap: map,
                title: "Disable cusltering feature",
                id: "mcluster",
                label: "Clustering",
                action: function () {
                    if (clusterbool) {
                        clusterbool = !clusterbool;
                    }
                    else {
                        clusterbool = !clusterbool;
                    }

                    if (clusterbool) {

                        jobmarkerCluster = new MarkerClusterer(map, arrowmarkersarr, {
                            maxZoom: 15,
                            zoomOnClick: false,
                            gridSize: gridSizes[map.getZoom()]
                        });

                    }
                    else {
                        if (jobmarkerCluster != null) {
                            jobmarkerCluster.clearMarkers();
                        }
                        setMaptoMarker();
                        SetArrowVehicleMarkers();
                    }

                }
            }
            check2 = new checkBox(checkOptions8);

            var checkOptions = {
                gmap: map,
                title: "Show map plateno text",
                id: "platenotext",
                label: "Plateno Text",
                action: function () {
                    if (Platenobool) {
                        Platenobool = !Platenobool;
                    }
                    else {
                        Platenobool = !Platenobool;
                    }
                    if (vehiclemarkercustomMapCanvas != undefined) {
                        vehiclemarkercustomMapCanvas.canvasDraw();
                    }
                }
            }
            check1 = new checkBox(checkOptions);

            var checkOptions3 = {
                gmap: map,
                title: "Show map truck distance from plant",
                id: "jobtext",
                label: "Distance Info",
                action: function () {
                    if (JobDescbool) {
                        JobDescbool = !JobDescbool;
                    }
                    else {
                        JobDescbool = !JobDescbool;
                    }
                    if (vehiclemarkercustomMapCanvas != undefined) {
                        vehiclemarkercustomMapCanvas.canvasDraw();
                    }
                }
            }
            check3 = new checkBox(checkOptions3);

            var ddDivOptions;

            ddDivOptions = {
                items: [check2, check1, check3],
                id: "myddOptsDiv"
            }

            var dropDownDiv = new dropDownOptionsDiv(ddDivOptions);

            var dropDownOptions = {
                gmap: map,
                name: "My Map",
                id: 'ddControl',
                title: "Map Options",
                position: google.maps.ControlPosition.TOP_RIGHT,
                dropDown: dropDownDiv
            }
            var dropDown1 = new dropDownControl(dropDownOptions);
            google.maps.event.addListener(map, 'zoom_changed', function () {
                //if (markerClusterer != null) {
                //    markerClusterer.setGridSize(gridSizes[map.getZoom()]);
                //}
                setMaptoMarker();
                HandleZoomArrowMakers();
            });

            google.maps.event.addListener(map, 'dragend', function (ev) {
                setMaptoMarker();
            });

            $("#panel2").hide();

            $(".btn-slide").click(function () {

                $("#panel").slideToggle("fast");

                var str = document.getElementById("udimg").src;
                if (str.indexOf("down") > 0) {
                    document.getElementById("udimg").src = "images/up.gif";
                    document.getElementById("udimg").alt = "Close vehicle search";
                    document.getElementById("udimg").title = "Close vehicle search";
                }
                else {
                    document.getElementById("udimg").src = "images/down.gif";
                    document.getElementById("udimg").alt = "Open vehicle search";
                    document.getElementById("udimg").title = "Open vehicle search";
                }

                $(this).toggleClass("active");

                return false;
            });

            $(".btn-slide2").click(function () {

                $("#panel2").slideToggle("fast");

                var str = document.getElementById("udimg2").src;
                if (str.indexOf("down") > 0) {
                    document.getElementById("udimg2").src = "images/up.gif";
                    document.getElementById("udimg2").alt = "Close vehicle search";
                    document.getElementById("udimg2").title = "Close vehicle search";
                }
                else {
                    document.getElementById("udimg2").src = "images/down.gif";
                    document.getElementById("udimg2").alt = "Open vehicle search";
                    document.getElementById("udimg2").title = "Open vehicle search";
                }

                $(this).toggleClass("active");

                return false;
            });

            $(".trigger").click(function () {
                $(".jaffa").toggle("fast");
                $(this).toggleClass("active");
                $("#panel2").slideToggle("fast");

                return false;
            });

            fnFeaturesInit();
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 15,
                "aaSorting": [[2, "asc"]],
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
                    { "bVisible": true, "bSortable": true, "sWidth": "130px", "aTargets": [1] },
                    { "bVisible": true, "bSortable": true, "sWidth": "130px", "sClass": "fontfix", "aTargets": [2] },
                    { "bVisible": true, "bSortable": false, "aTargets": [5] },

                ],
            });

            window.setTimeout(callme, 2000);

            $(window).resize(function () {

                $(".VehicleMarker_OVERLAY").attr("height", window.innerHeight);
                $(".VehicleMarker_OVERLAY").attr("width", window.innerWidth);

                mapsize = { width: window.innerWidth, height: window.innerHeight };

                if (vehiclemarkercustomMapCanvas != undefined) {
                    vehiclemarkercustomMapCanvas.canvasDraw();
                }

            });

        });
        var marker = [];
        var markerP;
        var LmarkersArray = [];
        var radius50;
        function SearchTrucks() {

            $("#floatingBarsG").removeClass("hideen");
            $("body").addClass("opacity");
            if ($("#ddlplant").val() != 0) {
                $.ajax({
                    type: "POST",
                    url: "PlantVehicleSearch.aspx/GetData",
                    data: '{Plantid: \"' + $("#ddlplant").val() + '\",Type:\"' + $("#ddltransporter").val() + '\",kmdist:\"' + $("#ddldistance").val() + '\",vtype:\"' + $("#ddlType").val() + '\"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        for (let i = 0; i < LmarkersArray.length; i++) {
                            LmarkersArray[i].setMap(null);
                        }
                        LmarkersArray.length = 0;
                        marker.length = 0;
                        if (radius50 != undefined) {
                            radius50.setMap(null);
                        }
                        if (markerP != undefined) {
                            markerP.setMap(null);
                        }

                        var json = response.aaData;
                        var plantname = $("#ddlplant option:selected").text();
                        var mpoint = new google.maps.LatLng(parseFloat(json[0].lat), parseFloat(json[0].lon));
                        markerP = new google.maps.Marker({
                            position: mpoint, map: map,

                            label: { color: '#000000', fontWeight: 'bold', fontSize: '14px', text: plantname + ' PLANT' }
                        });
                        markerP.setZIndex(9999);

                        map.setCenter(mpoint);
                        radius50 = new google.maps.Circle({
                            strokeColor: '#05FFD7',
                            strokeOpacity: 1,
                            strokeWeight: 1,
                            fillColor: '#05FFD7',
                            fillOpacity: 0.25,
                            clickable: false,
                            editable: false,
                            zIndex: 2,
                            map: map
                        });
                        radius50.setCenter(mpoint);
                        radius50.setRadius(50000);

                        table = oTable.dataTable();
                        table._fnProcessingDisplay(true);
                        oSettings = table.fnSettings();
                        table.fnClearTable(this);

                        var res = new Array();

                        for (var i = 1; i < json.length; i++) {
                            var plateno = json[i].plateno;
                            //mpoint = new google.maps.LatLng(parseFloat(json[i].lat), parseFloat(json[i].lon));
                            //marker[plateno] = new RichMarker({
                            //    draggable: false,
                            //    anchor: new google.maps.Size(-35, -35),
                            //    padding: "-2px -2px",
                            //    flat: true,

                            //});
                            //marker[plateno].setPosition(mpoint);
                            //var cont = "";
                            //var cls = "r";
                            //marker[plateno].setZIndex(9997);
                            //if (json[i].ignition && json[i].speed == 0) {
                            //    marker[plateno].setZIndex(9998);
                            //    cls = "b";
                            //}
                            //else {
                            //    marker[plateno].setZIndex(9999);
                            //    cls = "g";
                            //}
                            //cont += "<div class='" + cls + "' title='" + plateno + "'>" + plateno + "&nbsp;";
                            //cont += "<span class='pravin' title='Distance From Plant' style='background: White; color: Red;'>&nbsp;Distance&nbsp;- " + json[i].distance + " Km&nbsp;</span>";
                            //cont += "<div class='r-arrow-border'></div><div class='" + cls + "-arrow'></div></div></div>";
                            //marker[plateno].setContent(cont);
                            //LmarkersArray.push(marker[plateno]);

                            res.length = 0;
                            res.push(i);
                            res.push("<span style='cursor:pointer;text-decoration:underline;color:blue' onclick='SetMapToCenter(\"" + parseFloat(json[i].lat) + "\",\"" + parseFloat(json[i].lon) + "\")'>" + plateno + "</span> <i class='fa fa-copy' style='font-size:15px;margin-left:5px;cursor:pointer' onclick='CopyText(\"" + plateno + "\")' title='Copy Plateno' />");
                            res.push(json[i].trnsportername);
                            res.push(json[i].trnsportertype);
                            res.push(json[i].distance);
                            res.push(json[i].address);

                            table.oApi._fnAddData(oSettings, res);

                        }
                        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                        table._fnProcessingDisplay(false);
                        table.fnDraw();
                        AddPlatenoNewLbl(json);
                        $(".btn-slide").trigger("click");
                        $(".trigger").addClass("active");
                        $("#panel2").show();
                        $("#floatingBarsG").addClass("hideen");
                        $("body").removeClass("opacity");
                    },
                    failure: function (response) {
                        alert("Error");
                    }
                });
            }
            else {
                alert("Please Select Plant");
            }

        }

        function ClearGeos(sts) {
            if (data_layer != undefined) {
                if (sts == 0) {
                    data_layer.forEach(function (feature) {
                        if (feature.getProperty('type') == "polygon") {
                            data_layer.overrideStyle(feature, {
                                visible: false
                            });

                            //data_layer.setStyle({ visible: false });
                            //});
                        }
                    });
                }
                else {
                    data_layer.forEach(function (feature) {
                        if (feature.getProperty('type') == "polygon") {
                            data_layer.overrideStyle(feature, {
                                visible: true
                            });

                            //data_layer.setStyle({ visible: false });
                            //});
                        }
                    });
                }
            }

            // var data_layer = new google.maps.Data({ map: map });
            //data_layer.setStyle({ visible: false });
            //google
        }
        var arrowmarkersarr = new Array();
        function AddPlatenoNewLbl(marray) {
            if (jobmarkerCluster) {
                jobmarkerCluster.clearMarkers();
            }
            ClearArrowVehicleMarker();
            var icon = {
                path: arrowpath,
                fillColor: '#000000',
                fillOpacity: .6,
                anchor: new google.maps.Point(15, 0),
                strokeWeight: 0,
                scale: 1,
                rotation: 90
            }
            var cls = ".r";
            arrowmarkersarr.length = 0;
            var varscale = 1;
            var zoom = map.zoom;
            if (zoom <= 6) {
                varscale = 0.35;
            }

            else {
                switch (zoom) {
                    case 7:
                        varscale = 0.50;
                        break;
                    case 8:
                        varscale = 0.55;
                        break;
                    case 9:
                        varscale = 0.65;
                        break;
                    case 10:
                        varscale = 0.75;
                        break;
                    case 11:
                        varscale = 0.85;
                        break;
                    case 12:
                        varscale = 0.90;
                        break;
                    default:
                        varscale = 1;
                        break;
                }
            }

            for (var i = 1; i < marray.length; i++) {
                if (marray[i].ignition && marray[i].speed == 0) {
                    cls = ".b";
                    icon = {
                        path: arrowpath,
                        fillColor: '#0000BF80',
                        fillOpacity: 1,
                        anchor: new google.maps.Point(15, 0),
                        strokeWeight: 3,
                        strokeColor: '#0000FF',
                        scale: varscale,
                        rotation: marray[i].bearing
                    }
                }
                else if (marray[i].ignition && marray[i].speed > 0) {
                    cls = ".g";
                    icon = {
                        path: arrowpath,
                        fillColor: '#40BF40DF',
                        fillOpacity: 1,
                        anchor: new google.maps.Point(15, 0),
                        strokeWeight: 2,
                        strokeColor: '#008000',
                        scale: varscale,
                        rotation: marray[i].bearing
                    }
                }
                else {
                    cls = ".r";
                    icon = {
                        path: arrowpath,
                        fillColor: '#FF808060',
                        fillOpacity: 1,
                        anchor: new google.maps.Point(15, 0),
                        strokeWeight: 2,
                        strokeColor: '#FF0000',
                        scale: varscale,
                        rotation: marray[i].bearing
                    }
                }
                var cont = "<div class='" + cls + "' title='" + marray[i].plateno + "'>" + marray[i].plateno + "";
                var vartempma = new google.maps.Marker({
                    position: { lat: marray[i].lat, lng: marray[i].lon },
                    center: { lat: marray[i].lat, lng: marray[i].lon },
                    icon: icon,
                    customtext: marray[i].plateno,
                    custommessage: marray[i].distance
                    //  label: cont
                });
                vartempma.setMap(map);

                arrowmarkersarr.push(vartempma);
            }
            VehicleMarkers();

            if (clusterbool) {

                jobmarkerCluster = new MarkerClusterer(map, arrowmarkersarr, {
                    maxZoom: 15,
                    zoomOnClick: false,
                    gridSize: gridSizes[map.getZoom()]
                });
            }
        }

        function VehicleMarkers() {
            var VehicleMarkerCanvasOverlay = function (map) {
                this.canvas = document.createElement("CANVAS");
                this.canvas.className = "VehicleMarker_OVERLAY";
                this.canvas.height = window.innerHeight;
                this.canvas.width = window.innerWidth;
                this.ctx = null;
                this.map = map;
                this.projection = null;
                //  this.data = data;
                this.setMap(map);
            };
            VehicleMarkerCanvasOverlay.prototype = new google.maps.OverlayView();
            VehicleMarkerCanvasOverlay.prototype.onAdd = function () {
                this.getPanes().floatPane.appendChild(this.canvas);
                this.ctx = this.canvas.getContext("2d");
            };

            VehicleMarkerCanvasOverlay.prototype.drawLable = function (text, p, jobdesc) {
                if (map.getZoom() > 6) {
                    var strokeWeight = 1;
                    if (this.map.getMapTypeId() == "roadmap") {
                        this.ctx.strokeStyle = "#ffffff";
                        this.ctx.fillStyle = "#4343ef";
                        strokeWeight = 2;
                    }
                    else {
                        this.ctx.strokeStyle = "black";
                        this.ctx.fillStyle = "white";
                        strokeWeight = 2;
                    }

                    this.ctx.font = "bold 12px arial";

                    var newtext = "";
                    if (Platenobool) {
                        newtext = text;

                    }
                    else {
                        newtext = "";
                    }
                    if (JobDescbool) {
                        //newtext = jobdesc;
                        if (newtext == "") {
                            newtext = jobdesc + " Kmd away";
                        }
                        else {
                            newtext += "  - " + jobdesc + " Kms away";
                        }

                    }
                    if (newtext != "") {
                        /// draw text from top - makes life easier at the moment
                        this.ctx.textBaseline = 'top';

                        ///// color for background
                        //this.ctx.fillStyle = '#f50';

                        /// get width of text
                        var width = this.ctx.measureText(newtext).width;

                        /// draw background rect assuming height of font
                        this.ctx.fillRect(p.x, p.y, width + 15, 18);

                        ///// text color
                        this.ctx.fillStyle = '#ffffff';

                        /// draw text on top
                        this.ctx.fillText(newtext, p.x + 5, p.y + 4);

                        this.ctx.restore();
                    }
                    else {

                    }

                    //  drawTextBG(this.ctx, text, this.ctx.font, p.x, p.y);
                    // this.ctx.fillText(text, p.x, p.y);
                }
            };

            VehicleMarkerCanvasOverlay.prototype.canvasDraw = function () {
                if (!dragging) {
                    //   if (!DrawPOIcanvas) {
                    var projection = this.getProjection();
                    // Position Canvas
                    var centerPoint = projection.fromLatLngToDivPixel(this.map.getCenter());
                    this.canvas.style.left = (centerPoint.x - mapsize.width / 2) + "px";
                    this.canvas.style.top = (centerPoint.y - mapsize.height / 2) + "px";
                    // Clear Canvas
                    this.ctx.clearRect(0, 0, mapsize.width, mapsize.height);
                    var prevbound = 0;
                    var t1, t2, dist;
                    var flag = false;

                    for (var i = 0; i < arrowmarkersarr.length; i++) {

                        var marker = new google.maps.LatLng(arrowmarkersarr[i].position.lat(), arrowmarkersarr[i].position.lng());
                        if (map.getBounds().contains(marker) && (arrowmarkersarr[i].map != null)) {

                            this.drawLable(arrowmarkersarr[i].customtext, projection.fromLatLngToContainerPixel(new google.maps.LatLng(arrowmarkersarr[i].position.lat(), arrowmarkersarr[i].position.lng())), arrowmarkersarr[i].custommessage);

                        }
                    }
                }

            };
            vehiclemarkercustomMapCanvas = new VehicleMarkerCanvasOverlay(map);
            // vehiclemarkercustomMapCanvas.canvasDraw();

            VehicleMarkerCanvasOverlay.prototype.draw = vehiclemarkercustomMapCanvas.canvasDraw;

        }
        function ClearArrowVehicleMarker() {
            for (let i = 0; i < arrowmarkersarr.length; i++) {
                arrowmarkersarr[i].setMap(null);
            }
            arrowmarkersarr.length = 0;
            $(".VehicleMarker_OVERLAY").remove();
        }

        function SetArrowVehicleMarkers() {
            for (let i = 0; i < arrowmarkersarr.length; i++) {
                arrowmarkersarr[i].setMap(map);
            }
        }

        function HandleZoomArrowMakers() {
            var varscale = 1;
            var zoom = map.zoom;
            if (zoom <= 6) {
                varscale = 0.35;
            }

            else {
                switch (zoom) {
                    case 7:
                        varscale = 0.50;
                        break;
                    case 8:
                        varscale = 0.55;
                        break;
                    case 9:
                        varscale = 0.65;
                        break;
                    case 10:
                        varscale = 0.75;
                        break;
                    case 11:
                        varscale = 0.85;
                        break;
                    case 12:
                        varscale = 0.90;
                        break;
                    default:
                        varscale = 1;
                        break;
                }
            }

            for (let i = 0; i < arrowmarkersarr.length; i++) {
                var tmpicon = arrowmarkersarr[i].getIcon()
                tmpicon.scale = varscale;
                arrowmarkersarr[i].setIcon(tmpicon);

            }

        }

        function SetMapToCenter(lat, lon) {
            if (map != undefined) {
                var mpoint = new google.maps.LatLng(parseFloat(lat), parseFloat(lon));
                map.setCenter(mpoint);
                map.setZoom(15);
            }
        }

        function CopyText(restext) {
            copyTextToClipboard(restext);
            /* Alert the copied text */
            alert("Copied the text: " + restext);
        }

        function fallbackCopyTextToClipboard(text) {
            var textArea = document.createElement("textarea");
            textArea.value = text;
            textArea.style.position = "fixed";  //avoid scrolling to bottom
            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();

            try {
                var successful = document.execCommand('copy');
                var msg = successful ? 'successful' : 'unsuccessful';
                console.log('Fallback: Copying text command was ' + msg);
            } catch (err) {
                console.error('Fallback: Oops, unable to copy', err);
            }

            document.body.removeChild(textArea);
        }
        function copyTextToClipboard(text) {
            if (!navigator.clipboard) {
                fallbackCopyTextToClipboard(text);
                return;
            }
            navigator.clipboard.writeText(text).then(function () {
                console.log('Async: Copying to clipboard was successful!');
            }, function (err) {
                console.error('Async: Could not copy text: ', err);
            });
        }

        var maray;
        function setMaptoMarker() {
            bounds = map.getBounds();
            if (markerClusterer) {
                markerClusterer.clearMarkers();
            }
            for (var i = 0; i < LmarkersArray.length; i++) {
                var mkr = LmarkersArray[i];
                if (bounds.contains(new google.maps.LatLng(parseFloat(mkr.getPosition().lat()), parseFloat(mkr.getPosition().lng())))) {
                    if (LmarkersArray[i].getMap() != null) {

                    }
                    else {
                        LmarkersArray[i].setMap(map);
                    }
                }

                else {
                    LmarkersArray[i].setMap(null);
                }
            }
            //markerClusterer = new MarkerClusterer(map, LmarkersArray, {
            //    maxZoom: 15,
            //    zoomOnClick: false,
            //    gridSize: gridSizes[map.getZoom()]
            //});

        }

        function hidediv() {

            document.getElementById("udimg").src = "images/down.gif";
            document.getElementById("udimg").alt = "Open vehicle search";
            document.getElementById("udimg").title = "Open vehicle search";

            $('#panel').hide();
            return false;
        }

        function hidediv2() {

            document.getElementById("udimg2").src = "images/down.gif";
            document.getElementById("udimg2").alt = "Open vehicle search";
            document.getElementById("udimg2").title = "Open vehicle search";

            $('#panel2').hide();
            return false;
        }
    </script>
</body>
</html>
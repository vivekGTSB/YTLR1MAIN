<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.WaitingTimeMapNew" CodeBehind="WaitingTimeMapNew.aspx.vb" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" integrity="sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7" crossorigin="anonymous">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js" integrity="sha384-0mSbJDEHialfmuBBQP6A4Qrprq5OVfW37PRR3j5ELqxss1yVqOtnepnHVP9aJ7xS" crossorigin="anonymous"></script>
    <script src="https://maps.googleapis.com/maps/api/js?v=3&client=gme-zigbeeautomation&libraries=drawing&channel=YTL"></script>
    <script src="jsfiles/richmarker.js"></script>
    <script src="jsfiles/waitingtimemapnew.js?r=0.019818"></script>


    <style type="text/css">
        .panel {
            margin-bottom: 0px;
        }

        .panel-body {
            padding: 1px;
        }

        .label_content {
            position: relative;
            border-radius: 5px;
            padding: 5px;
            color: #ffffff;
            background-color: red;
            font-size: 16px;
            width: 100%;
            line-height: 16px;
            text-align: center;
        }

            .label_content:after {
                content: '';
                position: absolute;
                top: 100%;
                left: 50%;
                margin-left: -8px;
                width: 0;
                height: 0;
                border-top: solid 8px red;
                border-left: solid 8px transparent;
                border-right: solid 8px transparent;
            }

        .spncls {
            color: #fff;
            margin-top: 0;
            margin-bottom: 0;
            font-size: 16px;
            color: inherit;
        }
    </style>

</head>
<body style="margin: 0px;">
    <div class="container-fluid">
        <div class="row">
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">BC - YTL BATU CAVES (BC)  &nbsp; (<span class="spncls" id="spn1"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map1" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">BS - BUKIT SAGU &nbsp; (<span class="spncls" id="spn2"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map2" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">GPK - GELANG PATAH PLANT &nbsp; (<span class="spncls" id="spn3"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map3" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">KP - KAPAR POWER PLANT (PFA) &nbsp; (<span class="spncls" id="spn4"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map4" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">KT - KANTHAN PLANT &nbsp; (<span class="spncls" id="spn5"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map5" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">LK - LANGKAWI CEMENT PLANT &nbsp; (<span class="spncls" id="spn6"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map6" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">LM - LUMUT &nbsp; (<span class="spncls" id="spn7"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map7" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">PG1 - PASIR GUDANG CEMENT PLANT 1 &nbsp; (<span class="spncls" id="spn8"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map8" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">PG2 - PASIR GUDANG CEMENT PLANT 2 &nbsp; (<span class="spncls" id="spn9"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map9" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">PG3 - PASIR GUDANG CEMENT PLANT 3 &nbsp; (<span class="spncls" id="spn10"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map10" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">PR - YTL PERAK HANJOONG &nbsp; (<span class="spncls" id="spn11"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map11" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">RW - RAWANG PLANT &nbsp; (<span class="spncls" id="spn12"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map12" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">TB - TANJUNG BIN POWER SDN BHD &nbsp; (<span class="spncls" id="spn13"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map13" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">WP - WEST PORT TERMINAL &nbsp; (<span class="spncls" id="spn14"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map14" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">WP2 - WESTPORT2 &nbsp; (<span class="spncls" id="spn15"></span>)</h3>
                    </div>
                    <div class="panel-body">
                        <div id="map15" style="height: 400px;"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</body>
</html>




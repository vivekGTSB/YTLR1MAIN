<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.ClientDashBoard" Codebehind="ClientDashBoard.aspx.vb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Admin Information Board</title>
    <script src="https://code.jquery.com/jquery-3.2.1.min.js"
        integrity="sha256-hwg4gsxgFZhOsEEamdOYGBf13FyQuiTwlAQgxVSNgt4="
        crossorigin="anonymous"></script>
    <script src="https://code.highcharts.com/highcharts.js"></script>
    <script src="https://code.highcharts.com/modules/exporting.js"></script>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"
        rel="stylesheet" />
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css"
        rel="stylesheet" />
    <script>
        $(document).ready(function () {
            GetPBTData();
            GetPBPData();
            GetPBCData();
            GetPaBTData();
            GetPaBPData();
            GetPaBCData();
            GetDBTData();
            GetDBPData();
            GetDBCData();
            GetPieChartData();
            function GetPBTData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=1",
                    data: '{p:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var ObjData = new Array();
                        var TransporterArray = new Array();
                        var csdArray = new Array();
                        var cqArray = new Array();
                        var dtArray = new Array();
                        var dbArray = new Array();

                        var DataArray = new Array();
                        for (var i = 0; i < response.length; i++) {
                            TransporterArray.push(response[i].name);
                            csdArray.push(parseInt(response[i].csd));
                            cqArray.push(parseInt(response[i].cq));
                            dtArray.push(parseInt(response[i].dt));
                            dbArray.push(parseInt(response[i].db));
                        }


                        var mydata = { name: "Transparency", data: csdArray };
                        ObjData.push(mydata);
                        //var mydata1 = { name: "Quality", data: cqArray };
                        //ObjData.push(mydata1);
                        var mydata2 = { name: "Delivery Time", data: dtArray };
                        ObjData.push(mydata2);
                        var mydata3 = { name: "Driver Behaviour", data: dbArray };
                        ObjData.push(mydata3);




                        Highcharts.chart('PBT', {
                            chart: {
                                type: 'bar'
                            },
                            title: {
                                text: 'By Transporter'
                            },
                            xAxis: {
                                categories: TransporterArray
                            },
                            yAxis: {
                                min: 0,
                                title: {
                                    text: 'Promoter information'
                                }
                            },
                            legend: {
                                reversed: true
                            },
                            plotOptions: {
                                series: {
                                    stacking: 'normal'
                                }
                            },
                            series: ObjData
                        });
                    },
                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });

            }
            function GetPBPData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=2",
                    data: '{p:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var ObjData = new Array();
                        var TransporterArray = new Array();
                        var csdArray = new Array();
                        var cqArray = new Array();
                        var dtArray = new Array();
                        var dbArray = new Array();

                        var DataArray = new Array();
                        for (var i = 0; i < response.length; i++) {
                            TransporterArray.push(response[i].name);
                            csdArray.push(parseInt(response[i].csd));
                            cqArray.push(parseInt(response[i].cq));
                            dtArray.push(parseInt(response[i].dt));
                            dbArray.push(parseInt(response[i].db));
                        }


                        //var mydata = { name: "Transparency", data: csdArray };
                        //ObjData.push(mydata);
                        var mydata1 = { name: "Quality", data: cqArray };
                        ObjData.push(mydata1);
                        //var mydata2 = { name: "Delivery Time", data: dtArray };
                        //ObjData.push(mydata2);
                        //var mydata3 = { name: "Driver Behaviour", data: dbArray };
                        //ObjData.push(mydata3);




                        Highcharts.chart('PBP', {
                            chart: {
                                type: 'bar'
                            },
                            title: {
                                text: 'By Plant'
                            },
                            xAxis: {
                                categories: TransporterArray
                            },
                            yAxis: {
                                min: 0,
                                title: {
                                    text: 'Promoter information'
                                }
                            },
                            legend: {
                                reversed: true
                            },
                            plotOptions: {
                                series: {
                                    stacking: 'normal'
                                }
                            },
                            series: ObjData
                        });
                    },
                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });

            }
            function GetPBCData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=3",
                    data: '{p:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var ObjData = new Array();
                        var TransporterArray = new Array();
                        var csdArray = new Array();
                        var cqArray = new Array();
                        var dtArray = new Array();
                        var dbArray = new Array();

                        var DataArray = new Array();
                        for (var i = 0; i < response.length; i++) {
                            TransporterArray.push(response[i].name);
                            csdArray.push(parseInt(response[i].csd));
                            cqArray.push(parseInt(response[i].cq));
                            dtArray.push(parseInt(response[i].dt));
                            dbArray.push(parseInt(response[i].db));
                        }


                        var mydata = { name: "Transparency", data: csdArray };
                        ObjData.push(mydata);
                        var mydata1 = { name: "Quality", data: cqArray };
                        ObjData.push(mydata1);
                        var mydata2 = { name: "Delivery Time", data: dtArray };
                        ObjData.push(mydata2);
                        var mydata3 = { name: "Driver Behaviour", data: dbArray };
                        ObjData.push(mydata3);




                        Highcharts.chart('PBC', {
                            chart: {
                                type: 'bar'
                            },
                            title: {
                                text: 'By Customer'
                            },
                            xAxis: {
                                categories: TransporterArray
                            },
                            yAxis: {
                                min: 0,
                                title: {
                                    text: 'Promoter information'
                                }
                            },
                            legend: {
                                reversed: true
                            },
                            plotOptions: {
                                series: {
                                    stacking: 'normal'
                                }
                            },
                            series: ObjData
                        });
                    },
                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });

            }

            function GetPaBTData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=4",
                    data: '{p:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var ObjData = new Array();
                        var TransporterArray = new Array();
                        var csdArray = new Array();
                        var cqArray = new Array();
                        var dtArray = new Array();
                        var dbArray = new Array();

                        var DataArray = new Array();
                        for (var i = 0; i < response.length; i++) {
                            TransporterArray.push(response[i].name);
                            csdArray.push(parseInt(response[i].csd));
                            cqArray.push(parseInt(response[i].cq));
                            dtArray.push(parseInt(response[i].dt));
                            dbArray.push(parseInt(response[i].db));
                        }


                        var mydata = { name: "Transparency", data: csdArray };
                        ObjData.push(mydata);
                        //var mydata1 = { name: "Quality", data: cqArray };
                        //ObjData.push(mydata1);
                        var mydata2 = { name: "Delivery Time", data: dtArray };
                        ObjData.push(mydata2);
                        var mydata3 = { name: "Driver Behaviour", data: dbArray };
                        ObjData.push(mydata3);




                        Highcharts.chart('PaBT', {
                            chart: {
                                type: 'bar'
                            },
                            title: {
                                text: 'By Transporter'
                            },
                            xAxis: {
                                categories: TransporterArray
                            },
                            yAxis: {
                                min: 0,
                                title: {
                                    text: 'Passive information'
                                }
                            },
                            legend: {
                                reversed: true
                            },
                            plotOptions: {
                                series: {
                                    stacking: 'normal'
                                }
                            },
                            series: ObjData
                        });
                    },
                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });

            }
            function GetPaBPData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=5",
                    data: '{p:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var ObjData = new Array();
                        var TransporterArray = new Array();
                        var csdArray = new Array();
                        var cqArray = new Array();
                        var dtArray = new Array();
                        var dbArray = new Array();

                        var DataArray = new Array();
                        for (var i = 0; i < response.length; i++) {
                            TransporterArray.push(response[i].name);
                            csdArray.push(parseInt(response[i].csd));
                            cqArray.push(parseInt(response[i].cq));
                            dtArray.push(parseInt(response[i].dt));
                            dbArray.push(parseInt(response[i].db));
                        }


                        //var mydata = { name: "Transparency", data: csdArray };
                        //ObjData.push(mydata);
                        var mydata1 = { name: "Quality", data: cqArray };
                        ObjData.push(mydata1);
                        //var mydata2 = { name: "Delivery Time", data: dtArray };
                        //ObjData.push(mydata2);
                        //var mydata3 = { name: "Driver Behaviour", data: dbArray };
                        //ObjData.push(mydata3);




                        Highcharts.chart('PaBP', {
                            chart: {
                                type: 'bar'
                            },
                            title: {
                                text: 'By Plant'
                            },
                            xAxis: {
                                categories: TransporterArray
                            },
                            yAxis: {
                                min: 0,
                                title: {
                                    text: 'Passive information'
                                }
                            },
                            legend: {
                                reversed: true
                            },
                            plotOptions: {
                                series: {
                                    stacking: 'normal'
                                }
                            },
                            series: ObjData
                        });
                    },
                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });

            }
            function GetPaBCData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=6",
                    data: '{p:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var ObjData = new Array();
                        var TransporterArray = new Array();
                        var csdArray = new Array();
                        var cqArray = new Array();
                        var dtArray = new Array();
                        var dbArray = new Array();

                        var DataArray = new Array();
                        for (var i = 0; i < response.length; i++) {
                            TransporterArray.push(response[i].name);
                            csdArray.push(parseInt(response[i].csd));
                            cqArray.push(parseInt(response[i].cq));
                            dtArray.push(parseInt(response[i].dt));
                            dbArray.push(parseInt(response[i].db));
                        }


                        var mydata = { name: "Transparency", data: csdArray };
                        ObjData.push(mydata);
                        var mydata1 = { name: "Quality", data: cqArray };
                        ObjData.push(mydata1);
                        var mydata2 = { name: "Delivery Time", data: dtArray };
                        ObjData.push(mydata2);
                        var mydata3 = { name: "Driver Behaviour", data: dbArray };
                        ObjData.push(mydata3);




                        Highcharts.chart('PaBC', {
                            chart: {
                                type: 'bar'
                            },
                            title: {
                                text: 'By Customer'
                            },
                            xAxis: {
                                categories: TransporterArray
                            },
                            yAxis: {
                                min: 0,
                                title: {
                                    text: 'Passive information'
                                }
                            },
                            legend: {
                                reversed: true
                            },
                            plotOptions: {
                                series: {
                                    stacking: 'normal'
                                }
                            },
                            series: ObjData
                        });
                    },
                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });

            }

            function GetDBTData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=7",
                    data: '{p:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var ObjData = new Array();
                        var TransporterArray = new Array();
                        var csdArray = new Array();
                        var cqArray = new Array();
                        var dtArray = new Array();
                        var dbArray = new Array();

                        var DataArray = new Array();
                        for (var i = 0; i < response.length; i++) {
                            TransporterArray.push(response[i].name);
                            csdArray.push(parseInt(response[i].csd));
                            cqArray.push(parseInt(response[i].cq));
                            dtArray.push(parseInt(response[i].dt));
                            dbArray.push(parseInt(response[i].db));
                        }


                        var mydata = { name: "Transparency", data: csdArray };
                        ObjData.push(mydata);
                        //var mydata1 = { name: "Quality", data: cqArray };
                        //ObjData.push(mydata1);
                        var mydata2 = { name: "Delivery Time", data: dtArray };
                        ObjData.push(mydata2);
                        var mydata3 = { name: "Driver Behaviour", data: dbArray };
                        ObjData.push(mydata3);




                        Highcharts.chart('DBT', {
                            chart: {
                                type: 'bar'
                            },
                            title: {
                                text: 'By Transporter'
                            },
                            xAxis: {
                                categories: TransporterArray
                            },
                            yAxis: {
                                min: 0,
                                title: {
                                    text: 'Detractors information'
                                }
                            },
                            legend: {
                                reversed: true
                            },
                            plotOptions: {
                                series: {
                                    stacking: 'normal'
                                }
                            },
                            series: ObjData
                        });
                    },
                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });

            }
            function GetDBPData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=8",
                    data: '{p:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var ObjData = new Array();
                        var TransporterArray = new Array();
                        var csdArray = new Array();
                        var cqArray = new Array();
                        var dtArray = new Array();
                        var dbArray = new Array();

                        var DataArray = new Array();
                        for (var i = 0; i < response.length; i++) {
                            TransporterArray.push(response[i].name);
                            csdArray.push(parseInt(response[i].csd));
                            cqArray.push(parseInt(response[i].cq));
                            dtArray.push(parseInt(response[i].dt));
                            dbArray.push(parseInt(response[i].db));
                        }


                        //var mydata = { name: "Transparency", data: csdArray };
                        //ObjData.push(mydata);
                        var mydata1 = { name: "Quality", data: cqArray };
                        ObjData.push(mydata1);
                        //var mydata2 = { name: "Delivery Time", data: dtArray };
                        //ObjData.push(mydata2);
                        //var mydata3 = { name: "Driver Behaviour", data: dbArray };
                        //ObjData.push(mydata3);




                        Highcharts.chart('DBP', {
                            chart: {
                                type: 'bar'
                            },
                            title: {
                                text: 'By Plant'
                            },
                            xAxis: {
                                categories: TransporterArray
                            },
                            yAxis: {
                                min: 0,
                                title: {
                                    text: 'Detractors information'
                                }
                            },
                            legend: {
                                reversed: true
                            },
                            plotOptions: {
                                series: {
                                    stacking: 'normal'
                                }
                            },
                            series: ObjData
                        });
                    },
                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });

            }
            function GetDBCData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=9",
                    data: '{p:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var ObjData = new Array();
                        var TransporterArray = new Array();
                        var csdArray = new Array();
                        var cqArray = new Array();
                        var dtArray = new Array();
                        var dbArray = new Array();

                        var DataArray = new Array();
                        for (var i = 0; i < response.length; i++) {
                            TransporterArray.push(response[i].name);
                            csdArray.push(parseInt(response[i].csd));
                            cqArray.push(parseInt(response[i].cq));
                            dtArray.push(parseInt(response[i].dt));
                            dbArray.push(parseInt(response[i].db));
                        }


                        var mydata = { name: "Transparency", data: csdArray };
                        ObjData.push(mydata);
                        var mydata1 = { name: "Quality", data: cqArray };
                        ObjData.push(mydata1);
                        var mydata2 = { name: "Delivery Time", data: dtArray };
                        ObjData.push(mydata2);
                        var mydata3 = { name: "Driver Behaviour", data: dbArray };
                        ObjData.push(mydata3);




                        Highcharts.chart('DBC', {
                            chart: {
                                type: 'bar'
                            },
                            title: {
                                text: 'By Customer'
                            },
                            xAxis: {
                                categories: TransporterArray
                            },
                            yAxis: {
                                min: 0,
                                title: {
                                    text: 'Detractors information'
                                }
                            },
                            legend: {
                                reversed: true
                            },
                            plotOptions: {
                                series: {
                                    stacking: 'normal'
                                }
                            },
                            series: ObjData
                        });
                    },
                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });

            }


            function GetPieChartData() {
                $.ajax({
                    type: "POST",
                    url: "GetClientDashData.aspx?p=10",
                    data: '{param:1}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.length > 0) {
                            var ObjData = new Array();
                            var mydata = { name: "Transparency", y: parseInt(response[0].csd) };
                            ObjData.push(mydata);
                            var mydata1 = { name: "Quality", y: parseInt(response[0].cq) };
                            ObjData.push(mydata1);
                            var mydata2 = { name: "Delivery Time", y: parseInt(response[0].dt) };
                            ObjData.push(mydata2);
                            var mydata3 = { name: "Driver Behaviour", y: parseInt(response[0].db) };
                            ObjData.push(mydata3);

                            Highcharts.chart('myPieChart', {
                                chart: {
                                    plotBackgroundColor: null,
                                    plotBorderWidth: null,
                                    plotShadow: false,
                                    type: 'pie'
                                },
                                title: {
                                    text: 'Promoters % Share Information Board '
                                },
                                tooltip: {
                                    pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                                },
                                plotOptions: {
                                    pie: {
                                        allowPointSelect: true,
                                        cursor: 'pointer',
                                        dataLabels: {
                                            enabled: true,
                                            format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                                            style: {
                                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                                            }
                                        }
                                    }
                                },
                                series: [{
                                    name: 'Percentage Share',
                                    colorByPoint: true,
                                    data: ObjData
                                }]
                            });


                        }
                    },

                    failure: function (response) {
                        alertbox("Error!", "Failed");
                    }
                });


            }

            
        });

    </script>
    <style>
        .highcharts-credits
        {
            display:none !important;
            }
        .myheader {
            font-size: 20px;
            font-family: Verdana sans-serif;
        }

        .jumbotron {
            padding-top: 4px !important;
            padding-bottom: 12px !important;
            margin-bottom: 0px !important;
            color: inherit;
            background-color: #eee;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="jumbotron">
                <h3><i class="fa fa-tachometer text-white" aria-hidden="true"></i>Client Information
                    Board</h3>

            </div>
            <div class="panel panel-default">
                <div class="panel-body">

                    <div class="myheader label label-success"><i class="fa fa-smile-o" aria-hidden="true">
                    </i>&nbsp;Promoters Information Board</div>
                    <div class="row">
                        <div class="col-md-3 col-sm-12">
                            <div id="PBT" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto">
                            </div>
                        </div>
                        <div class="col-md-1">
                        </div>
                        <div class="col-md-3 col-sm-12">
                            <div id="PBP" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto">
                            </div>
                        </div>
                        <div class="col-md-1">
                        </div>
                        <div class="col-md-3 col-sm-12">
                            <div id="PBC" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto">
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="myheader label label-warning"><i class="fa fa-meh-o" aria-hidden="true">
                    </i>&nbsp;Passive Information Board</div>

                    <div class="row">
                        <div class="col-md-3 col-sm-12">
                            <div id="PaBT" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto">
                            </div>
                        </div>
                        <div class="col-md-1">
                        </div>
                        <div class="col-md-3 col-sm-12">
                            <div id="PaBP" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto">
                            </div>
                        </div>
                        <div class="col-md-1">
                        </div>
                        <div class="col-md-3 col-sm-12">
                            <div id="PaBC" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto">
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="myheader label label-danger"><i class="fa fa-frown-o" aria-hidden="true">
                    </i>&nbsp;Detractors Information Board</div>

                    <div class="row">
                        <div class="col-md-3 col-sm-12">
                            <div id="DBT" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto">
                            </div>
                        </div>
                        <div class="col-md-1">
                        </div>
                        <div class="col-md-3 col-sm-12">
                            <div id="DBP" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto">
                            </div>
                        </div>
                        <div class="col-md-1">
                        </div>
                        <div class="col-md-3 col-sm-12">
                            <div id="DBC" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto">
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="myheader label label-info"><i class="fa fa-heart-o" aria-hidden="true"></i>
                        &nbsp;Promoters % Share Information Board</div>
                   
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <div id="myPieChart" style="min-width: 310px; max-width: 800px; height: 300px; margin: 0 auto; margin-top:10px !important;">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>



    </form>
</body>
</html>

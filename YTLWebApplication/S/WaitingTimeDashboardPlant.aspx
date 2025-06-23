<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.WaitingTimeDashboardPlant" Codebehind="WaitingTimeDashboardPlant.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Waiting Time Dashboard Plant</title>

    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript">
        google.charts.load('current', { packages: ['corechart', 'bar'] });
    </script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" integrity="sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7" crossorigin="anonymous">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js" integrity="sha384-0mSbJDEHialfmuBBQP6A4Qrprq5OVfW37PRR3j5ELqxss1yVqOtnepnHVP9aJ7xS" crossorigin="anonymous"></script>
  
    
     
  
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
    </style>
</head>
<body style="margin: 0px; ">
    <form id="form1" runat="server">
     <div class="container-fluid">
        <div class="row">
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">RW - Rawang Works</h3>
                    </div>

                    <div class="panel-body">
                        <div id="container1" style="height: 300px;"></div>
                    </div>
                </div>
            </div> 
          <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">KW - Kanthan Works</h3>
                    </div>

                    <div class="panel-body">
                        <div id="container2" style="height: 300px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px;">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">WPT - West Port Teminal</h3>
                    </div>
                    <div class="panel-body">
                        <div id="container3" style="height: 300px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px;">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">PGT - Pasir Gudang Terminal</h3>
                    </div>

                    <div class="panel-body">
                        <div id="container4" style="height: 300px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px;">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">HM - Holcim Malaysia</h3>
                    </div>

                    <div class="panel-body">
                        <div id="container5" style="height: 300px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px;">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">SA - Shah Alam</h3>
                    </div>

                    <div class="panel-body">
                        <div id="container6" style="height: 300px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">LW - Langkawi Works</h3>
                    </div>

                    <div class="panel-body">
                        <div id="container7" style="height: 300px;"></div>
                    </div>
                </div>
            </div>

            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px;">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">TBIN - Tanjung Bin</h3>
                    </div>
                    <div class="panel-body">
                        <div id="container8" style="height: 300px;"></div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4" style="padding: 5px;">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title">SG - Singapore</h3>
                    </div>

                    <div class="panel-body">
                        <div id="container9" style="height: 300px;"></div>
                    </div>
                </div>
            </div>

       
       
    </div>
    </div> 
    <script type="text/javascript"  language="JavaScript">
        function drawChart() {
            $.getJSON('GetChartData.aspx?r=' + Math.random(), null, function (json) {

                for (var i = 0; i < json.aaData.length; i++) {
                    // table.oApi._fnAddData(oSettings, json.aaData[i]);





                    var data = google.visualization.arrayToDataTable([
               ['Hours', 'Trucks'],
               ['5+', json.aaData[i][11]],
               ['5', json.aaData[i][10]],
               ['4.5', json.aaData[i][9]],
               ['4', json.aaData[i][8]],
               ['3.5', json.aaData[i][7]],
               ['3', json.aaData[i][6]],
               ['2.5', json.aaData[i][5]],
               ['2', json.aaData[i][4]],
               ['1.5', json.aaData[i][3]],
               ['1', json.aaData[i][2]]
            ]);

                    var view = new google.visualization.DataView(data);
                    view.setColumns([0, 1,
                             {
                                 calc: "stringify",
                                 sourceColumn: 1,
                                 type: "string",
                                 role: "annotation"
                             }
                             ]);

                    var options = {
                      //  title: json.aaData[i][0] + " - " + json.aaData[i][1],
                        titlePosition: 'center',
                        legend: { position: 'none' },
                        vAxis: {
                            title: 'Trucks',
                            minValue: 0,
                            ticks: [0, 1, 2, 3, 4, 5]
                        },
                        hAxis: {
                            title: 'Hours'
                        }

                    };
                    if (json.aaData[i][0] == "LW") {
                        var name = "container7";
                        var chart1 = new google.visualization.ColumnChart(document.getElementById(name));
                        chart1.draw(view, options);
                    }
                    if (json.aaData[i][0] == "WPT") {
                        var name = "container3";
                        var chart1 = new google.visualization.ColumnChart(document.getElementById(name));
                        chart1.draw(view, options);
                    }
                    else {
                        var name = "container" + (i + 1);
                        var chart1 = new google.visualization.ColumnChart(document.getElementById(name));
                        chart1.draw(view, options);
                    }











                }

            });

    }

    google.charts.setOnLoadCallback(drawChart);
    $(window).resize(function () {
        if (this.resizeTO) clearTimeout(this.resizeTO);
        this.resizeTO = setTimeout(function () {
            $(this).trigger('resizeEnd');
        }, 500);
    });

    //redraw graph when window resize is completed  
    $(window).on('resizeEnd', function () {
        drawChart();
    });
    </script>
     <script type="text/javascript">
         window.setInterval("drawChart()", 60000);
    </script>
    </form>
</body>
</html>

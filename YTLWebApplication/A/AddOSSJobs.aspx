<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AddOSSJobs.aspx.vb" Inherits="YTLWebApplication.AddOSSJobs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0, minimal-ui" />
    <meta name="description" content="YTLOSS,YTL,Cement" />
    <meta name="keywords" content="OSS JOBS" />
    <meta name="author" content="Gussmann" />
    <title>YTL-OSS JObs</title>
    <link rel="apple-touch-icon" href="dashboard/app-assets/images/ico/logo.png" />
    <link rel="shortcut icon" type="image/x-icon" href="dashboard/app-assets/images/ico/logo.ico" />
    <link href="https://fonts.googleapis.com/css?family=Montserrat:300,400,500,600" rel="stylesheet" />

    <!-- BEGIN: Vendor CSS-->
    <link rel="stylesheet" type="text/css" href="dashboard/app-assets/vendors/css/vendors.min.css" />
    <link href="dashboard/app-assets/datetimepicker/css/bootstrap-datetimepicker.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="dashboard/app-assets/vendors/css/extensions/sweetalert2.min.css">
    <!-- END: Vendor CSS-->

    <!-- BEGIN: Theme CSS-->
    <link rel="stylesheet" type="text/css" href="dashboard/app-assets/css/bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="dashboard/app-assets/css/bootstrap-extended.css" />
    <link rel="stylesheet" type="text/css" href="dashboard/app-assets/css/colors.css" />
    <link rel="stylesheet" type="text/css" href="dashboard/app-assets/css/components.css" />
    <link rel="stylesheet" type="text/css" href="dashboard/app-assets/css/themes/dark-layout.css" />
    <link rel="stylesheet" type="text/css" href="dashboard/app-assets/css/themes/semi-dark-layout.css" />




    <style>
        body.dark-layout .header-navbar-shadow {
            background: linear-gradient(180deg, rgba(44, 48, 60, .9) 4%, rgba(44, 48, 60, .43) 3%, rgba(44, 48, 60, 0));
        }

        html body .content .content-wrapper {
            margin-top: 1rem;
        }

        html body .content {
            margin-left: 0px;
        }
    </style>
</head>
<body class="vertical-layout vertical-menu-modern dark-layout 2-columns  navbar-floating footer-static  " data-open="click" data-menu="vertical-menu-modern" data-col="2-columns" data-layout="dark-layout">
    <form id="form1" runat="server">
        <div class="content-wrapper">
            <div class="content-body">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Add Jobs To OSS</h4>

                    </div>
                    <div class="card-content">
                        <div class="card-body card-dashboard">
                            <div class="row">
                                <div class="col-md-6 col-md-6 col-sm-6 col-sm-6">
                                    <div class="form-group">
                                        <label for="first-name-vertical">DN No</label>
                                        <asp:TextBox ID="txtdn_no" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">DN ID</label>
                                        <asp:TextBox ID="txtdn_id" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">Weightout Datetime</label>
                                        <asp:TextBox ID="datetimepicker1" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">Plateno</label>
                                        <asp:TextBox ID="txtplateno" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                    <div class="form-group">
                                        <label for="first-name-vertical">Plant</label>
                                        <asp:DropDownList ID="ddlplant" CssClass="form-control" runat="server">
                                            <asp:ListItem Value="ASSB">ASSB Alliance Steel SDN BHD</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">Destination Site</label>
                                        <asp:DropDownList ID="ddldestination" CssClass="form-control" runat="server">
                                            <asp:ListItem Value="0">Select Destination</asp:ListItem>
                                            <asp:ListItem Value="48159" data-area="1156">Buildcon Concrete Sdn Bhd- 48159 (JALAN TUN RAZAK)</asp:ListItem>
                                            <asp:ListItem Value="15615"  data-area="40">C.I READYMIX SDN BHD- 15615 (SENTUL)</asp:ListItem>
                                            <asp:ListItem Value="76028" data-area="39">Lafarge Concrete (M) - Chan Sow Lin- 76028 (SUNGAI BESI)</asp:ListItem>
                                            <asp:ListItem Value="76427" data-area="78">Lafarge Concrete (M) - PJ Plant- 76427 (PETALING JAYA)</asp:ListItem>
                                            <asp:ListItem Value="76811" data-area="1131">Lafarge Concrete (Malaysia) Sdn Bhd- 76811 (BUKIT BINTANG)</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">Transporter</label>
                                        <asp:DropDownList ID="ddltransporter" CssClass="form-control" runat="server">
                                        </asp:DropDownList>
                                    </div>

                                </div>
                                    </div>
                                <div class="col-md-6 col-md-6 col-sm-6 col-sm-6">
                                    <div class="form-group">
                                        <label for="first-name-vertical">Driver Name</label>
                                        <asp:TextBox ID="txtdrivername" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">Driver IC</label>
                                        <asp:TextBox ID="txtdriverid" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">DN Qty</label>
                                        <asp:TextBox ID="txtdnqty" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">Area</label>
                                        <asp:DropDownList ID="dllarea" CssClass="form-control" runat="server">
                                            <asp:ListItem Value="1131">BUKIT BINTANG</asp:ListItem>
                                            <asp:ListItem Value="1156">JALAN TUN RAZAK</asp:ListItem>
                                            <asp:ListItem Value="39">SUNGAI BESI</asp:ListItem>
                                            <asp:ListItem Value="40">SENTUL</asp:ListItem>
                                            <asp:ListItem Value="78">PETALING JAYA</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">Product</label>
                                        <asp:DropDownList ID="ddlproduct" CssClass="form-control" runat="server">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="first-name-vertical">Order No</label>
                                        <asp:TextBox ID="txtorderno" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <center>
                                            <asp:Label runat ="server"  ID="alertbox" style="color :red"> Test Data</asp:Label> <br />
                                       <input type ="button" class ="btn btn-success" value ="Submit" onclick="mysubmit()"/>     
                                      
                                            </center>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>


            </div>
        </div>
    </form>
    <script src="dashboard/app-assets/js/core/libraries/jquery.min.js"></script>
    <script src="dashboard/app-assets/js/core/libraries/bootstrap.min.js"></script>
    <script src="dashboard/app-assets/datetimepicker/js/bootstrap-datetimepicker.js"></script>
    <script src="dashboard/app-assets/vendors/js/extensions/sweetalert2.all.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('#datetimepicker1').datetimepicker();
            $("#ddldestination").change(function () {
                $("#dllarea").val($("#ddldestination option:selected").attr("data-area"));
            });
        });
        function Validate() {
            if ($("#txtdn_no").val() == "") {
                AlertBox("Please Enter DN Number");
                return false;
            }
            if ($("#txtdn_id").val() == "") {
                AlertBox("Please Enter DN ID");
                return false;
            }
            if ($("#datetimepicker1").val() == "") {
                AlertBox("Please Select Date");
                return false;
            }
            if ($("#txtplateno").val() == "") {
                AlertBox("Please Enter Plateno");
                return false;
            }
            if ($("#ddldestination").val() == 0) {
                AlertBox("Please Select Destination Site");
                return false;
            }
            if ($("#ddltransporter").val() == 0) {
                AlertBox("Please Select Transporter");
                return false;
            }
            if ($("#txtdrivername").val() == "") {
                AlertBox("Please Enter Drivername");
                return false;
            }
            if ($("#txtdriverid").val() == "") {
                AlertBox("Please Enter Driver IC");
                return false;
            }
            if ($("#txtdnqty").val() == "") {
                AlertBox("Please Enter DN Quantity");
                return false;
            }
            if ($("#ddlproduct").val() == 0) {
                AlertBox("Please Select Product");
                return false;
            }
            if ($("#txtorderno").val() == "") {
                AlertBox("Please Enter Order Number");
                return false;
            }
            return true;
        }
        function mysubmit() {
            if (Validate()) {
                $.ajax({
                    type: "POST",
                    url: "AddOSSJobs.aspx/AddJob",
                    contentType: "application/json; charset=utf-8",
                    data: '{ dnno:\"' + escape($("#txtdn_no").val()) + '\",dnid:\"' + escape($("#txtdn_id").val()) + '\",weightouttime:\"' + $("#datetimepicker1").val() + '\",plateno:\"' + escape($("#txtplateno").val()) + '\",plant:\"' + $("#ddlplant").val() + '\",destinationsiteid:\"' + $("#ddldestination").val() + '\",destinationsite:\"' + escape($("#ddldestination option:selected").text()) + '\",trnasporterid:\"' + $("#ddltransporter").val() + '\",trnasporter:\"' + escape($("#ddltransporter option:selected").text()) + '\",drivername:\"' + escape($("#txtdrivername").val()) + '\",driveric:\"' + escape($("#txtdriverid").val()) + '\",dnqty:\"' + $("#txtdnqty").val() + '\",areaid:\"' + $("#dllarea").val() + '\",area:\"' + escape($("#dllarea option:selected").text()) + '\",productid:\"' + $("#ddlproduct").val() + '\",product:\"' + escape($("#ddlproduct option:selected").text()) + '\",orderno:\"' + escape($("#txtorderno").val()) + '\"}',
                    dataType: "json",
                    success: function (response) {
                        if (response.d.Code == 50) {
                            AlertBox("Unable to process request, Please try again");
                        }
                        else {
                            AlertBox(response.d.Response);
                            if (response.d.Code == 3) {
                                $("input[type=text]").val("");
                                $("#ddldestination").val(0);
                                $("#ddltransporter").val(0);
                                $("#ddlproduct").val(0);
                            }

                        }
                    },
                    error: function (request, status, error) {
                        console.log("GetData:" + error);
                    }
                });
            }
        }
        function AlertBox(Text) {
            Swal.fire({
                title: Text,
                confirmButtonClass: "btn btn-primary",
                buttonsStyling: !1
            })
        }
    </script>
</body>
</html>

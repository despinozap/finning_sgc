<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditarAccionInmediata.aspx.cs" Inherits="NCCSAN.Registros.EditarAccionInmediata" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .Popup_background
        {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }
        
        .Popup_frontground
        {
            background-color: White;
            padding: 30px; /*border-style: dotted;
            border-width: 2px;*/
            box-shadow: 10px 10px 5px #555555;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            setDatepickerStyle();
        });


        function setDatepickerStyle() {

        }

        function setDatepickerStyle() {

            $("#<%= txtFechaAccionInmediata.ClientID %>").datepicker
                (
                    {
                        // Formato de la fecha
                        dateFormat: 'dd-mm-yy',
                        // Primer dia de la semana El lunes
                        firstDay: 1,
                        // Dias Largo en castellano
                        dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
                        // Dias cortos en castellano
                        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                        // Nombres largos de los meses en castellano
                        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                        // Nombres de los meses en formato corto 
                        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dec"],
                        firstDay: 1
                    }
                );

            loadDatepickerStyle();

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

            function EndRequestHandler(sender, args) {
                $("#<%= txtFechaAccionInmediata.ClientID %>").datepicker
                (
                    {
                        // Formato de la fecha
                        dateFormat: 'dd-mm-yy',
                        // Primer dia de la semana El lunes
                        firstDay: 1,
                        // Dias Largo en castellano
                        dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
                        // Dias cortos en castellano
                        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                        // Nombres largos de los meses en castellano
                        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                        // Nombres de los meses en formato corto 
                        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dec"],
                        firstDay: 1
                    }
                );

                loadDatepickerStyle();
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Registro Acción Inmediata</h1>
    </div>
    <asp:HiddenField ID="hfCodigoEvento" runat="server" />
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table align="center">
                <tr>
                    <td>
                        <asp:ImageButton ID="ibVerDetalleEvento" runat="server" CssClass="submitPanelButton"
                            ImageUrl="~/Images/Button/view_evento.png" OnClick="ibVerDetalleEvento_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Panel ID="pnCausas" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td colspan="2" class="tdTitle">
                            <h1>
                                Panel de Causas</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" width="30%">
                            <asp:Label ID="Label31" runat="server" Text="Origen de falla" Font-Bold="False"></asp:Label>
                            &nbsp;<asp:Label ID="Label30" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" align="left" width="70%">
                            <asp:DropDownList ID="ddlOrigenFalla" runat="server" Width="230px" DataSourceID="SDSOrigen"
                                DataTextField="nombre" DataValueField="nombre" 
                                OnDataBound="ddlOrigenFalla_DataBound" AutoPostBack="True" 
                                onselectedindexchanged="ddlOrigenFalla_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSOrigen" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT [nombre] FROM [Origen]"></asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" width="30%">
                            <asp:Label ID="Label26" runat="server" Text="Tipo causa inmediata"></asp:Label>
                            &nbsp;<asp:Label ID="Label27" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" align="left" width="70%">
                            <asp:DropDownList ID="ddlTipoCausaInmediata" runat="server" AutoPostBack="True" 
                                Width="230px" DataTextField="tipo" 
                                DataValueField="tipo" ondatabound="ddlTipoCausaInmediata_DataBound" 
                                onselectedindexchanged="ddlTipoCausaInmediata_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSTipoCausaInmediata" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" 
                                SelectCommand="SELECT DISTINCT [tipo] FROM [CausaInmediata] WHERE ([tipo] &lt;&gt; @tipo)">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="N/A" Name="tipo" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" width="30%">
                            <asp:Label ID="Label4" runat="server" Text="Causa inmediata"></asp:Label>
                            &nbsp;<asp:Label ID="Label5" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" align="left" width="70%">
                            <asp:DropDownList ID="ddlCausaInmediata" runat="server" AutoPostBack="True" DataSourceID="SDSCausaInmediata"
                                DataTextField="nombre" DataValueField="nombre" OnDataBound="ddlCausaInmediata_DataBound"
                                OnSelectedIndexChanged="ddlCausaInmediata_SelectedIndexChanged" Width="230px">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSCausaInmediata" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT [nombre] FROM [CausaInmediata] WHERE ([tipo]=@tipo)">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlTipoCausaInmediata" Name="tipo" 
                                        PropertyName="SelectedValue" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label6" runat="server" Text="Sub-causa inmediata"></asp:Label>
                            &nbsp;<asp:Label ID="Label7" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:DropDownList ID="ddlSubcausaInmediata" runat="server" DataSourceID="SDSSubcausaInmediata"
                                DataTextField="nombre_subcausainmediata" DataValueField="nombre_subcausainmediata"
                                OnDataBound="ddlSubcausaInmediata_DataBound" Width="230px">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSSubcausaInmediata" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT nombre_subcausainmediata FROM CausaInmediataSubcausaInmediata WHERE nombre_causainmediata=@nombre_causainmediata">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlCausaInmediata" Name="nombre_causainmediata"
                                        PropertyName="SelectedValue" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label18" runat="server" Text="Tipo causa básica" Font-Bold="False"></asp:Label>
                            &nbsp;<asp:Label ID="Label19" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:DropDownList ID="ddlTipoCausaBasica" runat="server" Width="230px" AutoPostBack="True"
                                DataTextField="tipo" DataValueField="tipo" OnDataBound="ddlTipoCausaBasica_DataBound"
                                OnSelectedIndexChanged="ddlTipoCausaBasica_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSTipoCausaBasica" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [tipo] FROM [CausaBasica] WHERE ([tipo] &lt;&gt; @tipo)">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="N/A" Name="tipo" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label9" runat="server" Text="Causa básica" Font-Bold="False"></asp:Label>
                            &nbsp;<asp:Label ID="Label8" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:DropDownList ID="ddlCausaBasica" runat="server" AutoPostBack="True" DataTextField="nombre"
                                DataValueField="nombre" OnDataBound="ddlCausaBasica_DataBound" OnSelectedIndexChanged="ddlCausaBasica_SelectedIndexChanged"
                                Width="230px" DataSourceID="SDSCausaBasica">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSCausaBasica" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT [nombre] FROM [CausaBasica] WHERE ([tipo]=@tipo)">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlTipoCausaBasica" Name="tipo" PropertyName="SelectedValue" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="left">
                            <asp:Label ID="Label10" runat="server" Text="Sub-causa básica"></asp:Label>
                            &nbsp;<asp:Label ID="Label11" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlSubcausaBasica" runat="server" DataSourceID="SDSSubcausaBasica"
                                DataTextField="nombre_subcausabasica" DataValueField="nombre_subcausabasica"
                                OnDataBound="ddlSubcausaBasica_DataBound" Width="230px">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSSubcausaBasica" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT nombre_subcausabasica FROM CausaBasicaSubcausaBasica WHERE nombre_causabasica=@nombre_causabasica">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlCausaBasica" Name="nombre_causabasica" PropertyName="SelectedValue" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label16" runat="server" Text="Responsable de Evento" Font-Bold="False"></asp:Label>
                            &nbsp;<asp:Label ID="Label17" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:DropDownList ID="ddlResponsableEvento" runat="server" Width="230px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <asp:Panel ID="pnAccionInmediata" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td colspan="2" class="tdTitle">
                            <h1>
                                Panel de Acción Inmediata</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" width="20%">
                            <asp:Label ID="Label22" runat="server" Text="Acción inmediata"></asp:Label>
                            &nbsp;<asp:Label ID="Label42" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" width="80%">
                            <asp:TextBox ID="txtAccionInmediata" runat="server" Width="100%" Height="113px" MaxLength="3000"
                                TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label23" runat="server" Text="Fecha"></asp:Label>
                            &nbsp;<asp:Label ID="Label1" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtFechaAccionInmediata" runat="server" Width="190px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label20" runat="server" Text="Efectividad"></asp:Label>
                            &nbsp;<asp:Label ID="Label3" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:RadioButtonList ID="rblEfectividad" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem>Si</asp:ListItem>
                                <asp:ListItem>No</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label25" runat="server" Text="Observación"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtObservacion" runat="server" Height="113px" MaxLength="3000" TextMode="MultiLine"
                                Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <asp:Panel ID="pnArchivos" runat="server">
                    <table align="center" class="tableControls">
                        <tr>
                            <td colspan="2" class="tdTitle">
                                <h1>
                                    Panel de Archivos</h1>
                            </td>
                        </tr>
                        <tr>
                            <td class="tdFirst">
                                <asp:GridView ID="gvArchivos" runat="server" AutoGenerateColumns="False" Width="963px"
                                    OnRowCommand="gvArchivos_RowCommand" HorizontalAlign="Center" ShowHeaderWhenEmpty="True">
                                    <AlternatingRowStyle BackColor="#E4E4E4" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Nombre">
                                            <ItemTemplate>
                                                <asp:Label ID="lbNombre" runat="server"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tipo">
                                            <ItemTemplate>
                                                <asp:Label ID="lbTipo" runat="server"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tamaño">
                                            <ItemTemplate>
                                                <asp:Label ID="lbSize" runat="server"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Right" />
                                        </asp:TemplateField>
                                        <asp:ButtonField ButtonType="Image" CommandName="RemoveArchivo" HeaderText="Eliminar"
                                            ImageUrl="~/Images/Icon/remove_file.png">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:ButtonField>
                                    </Columns>
                                    <HeaderStyle BackColor="#666666" ForeColor="White" />
                                    <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <EmptyDataTemplate>
                                        <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                        <asp:Label ID="lbEmptyList" runat="server" Text="  No se han cargado archivos" Font-Size="14px"></asp:Label>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:FileUpload ID="fuArchivo" runat="server" Width="500" multiple="true" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tdLast" align="center">
                                <asp:ImageButton ID="ibAddArchivo" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Button/attachment.png"
                                    OnClick="ibAddArchivo_Click" CssClass="submitAttachFile" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <br />
                <asp:Panel ID="pnInvolucrados" runat="server">
                    <table align="center" class="tableControls">
                        <tr>
                            <td colspan="2" class="tdTitle">
                                <h1>
                                    Panel de Involucrados en Evento</h1>
                            </td>
                        </tr>
                        <tr>
                            <td class="tdFirst">
                                <asp:UpdatePanel ID="upInvolucrados" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="gvInvolucrados" runat="server" AutoGenerateColumns="False" HorizontalAlign="Center"
                                            OnRowCommand="gvInvolucrados_RowCommand" ShowFooter="True" Width="690px" ShowHeaderWhenEmpty="True">
                                            <Columns>
                                                <asp:TemplateField HeaderText="RUT">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbRUT" runat="server" Text=""></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nombre">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbNombre" runat="server" Text=""></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterStyle HorizontalAlign="Center" />
                                                    <FooterTemplate>
                                                        <asp:Button ID="btAdd" runat="server" Text="+ Agregar" CommandName="BuscarInvolucrado" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Centro">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbNombreCentro" runat="server" Text=""></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cargo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbCargo" runat="server" Text=""></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Antigüedad">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbAntiguedad" runat="server" Text=""></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="DelInvolucrado" HeaderText="Eliminar" ButtonType="Image"
                                                    ImageUrl="~/Images/Icon/remove_user.png">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <HeaderStyle BackColor="#666666" ForeColor="White" />
                                            <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            <EmptyDataTemplate>
                                                <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                                <asp:Label ID="lbEmptyList" runat="server" Text="  No se han cargado personas" Font-Size="14px"></asp:Label>
                                                <br />
                                                <asp:Button ID="btAdd" runat="server" Text="+ Agregar" CommandName="BuscarInvolucrado" />
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td>
                            <asp:ImageButton ID="ibVolver" runat="server" CssClass="submitPanelButton" ImageUrl="~/Images/Button/back.png"
                                OnClick="ibVolver_Click" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ibRegistrarAccionInmediata" runat="server" CssClass="submitPanelButton"
                                ImageUrl="~/Images/Button/save.png" OnClick="ibRegistrarAccionInmediata_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div align="left" style="color: #FF0000; text-align: center">
                <asp:Literal ID="ltSummary" runat="server"></asp:Literal>
            </div>
            <!-- Panel Detalle Evento -->
            <asp:HiddenField ID="hfDetalleEvento" runat="server" />
            <asp:ModalPopupExtender ID="hfDetalleEvento_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" PopupControlID="upDetalleEvento" TargetControlID="hfDetalleEvento">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upDetalleEvento" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnDetalleEvento" runat="server" HorizontalAlign="Center" CssClass="Popup_frontground"
                        ScrollBars="Auto" Height="500">
                        <table class="tableControls" align="center">
                            <tr>
                                <td colspan="5" class="tdTitle">
                                    <h1>
                                        Detalle del Evento</h1>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label52" runat="server" Text="Código" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label53" runat="server" Text="W/O" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label54" runat="server" Text="Cliente" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label55" runat="server" Text="Fuente" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoCodigo" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoWO" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreCliente" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreFuente" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label56" runat="server" Text="Fecha identificación" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label57" runat="server" Text="Tipo" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label58" runat="server" Text="Modelo" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label59" runat="server" Text="Serie equipo" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoFecha" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoTipoEquipo" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoModeloEquipo" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoSerieEquipo" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label60" runat="server" Text="Sistema" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label61" runat="server" Text="Sub-sistema" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label62" runat="server" Text="Componente" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label63" runat="server" Text="Serie componente" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreSistema" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreSubsistema" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreComponente" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoSerieComponente" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label64" runat="server" Text="Parte o Pieza" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label65" runat="server" Text="Número parte" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label66" runat="server" Text="Horas" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label67" runat="server" Text="" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoParte" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNumeroParte" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoHoras" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEvento" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label68" runat="server" Text="Área" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label69" runat="server" Text="Sub-área" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label70" runat="server" Text="Clasificación" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label71" runat="server" Text="Sub-clasificación" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreArea" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreSubarea" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreClasificacion" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreSubclasificacion" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label72" runat="server" Text="Probabilidad" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label73" runat="server" Text="Consecuencia" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label74" runat="server" Text="IRC" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label75" runat="server" Text="Criticidad" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoProbabilidad" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoConsecuencia" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoIRC" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoCriticidad" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label76" runat="server" Text="Detalle" Font-Bold="True"></asp:Label>
                                </td>
                                <td colspan="4" class="tdUnique" align="left">
                                    <asp:Label ID="lbDetalleEventoDetalle" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique" align="left">
                                    <asp:Label ID="Label28" runat="server" Text="Creado por" Font-Bold="True"></asp:Label>
                                </td>
                                <td colspan="4" class="tdUnique" align="left">
                                    <asp:LinkButton ID="lbtDetalleEventoDetalleCreador" runat="server" OnClick="lbtDetalleEventoDetalleCreador_Click"></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <br />
                        <table class="tableControls" align="center">
                            <tr>
                                <td class="tdTitle">
                                    <h1>
                                        Archivos adjuntos</h1>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:GridView ID="gvArchivosEvento" runat="server" AutoGenerateColumns="False" HorizontalAlign="Center"
                                        Width="863px" OnRowCommand="gvArchivosEvento_RowCommand" ShowHeaderWhenEmpty="True">
                                        <AlternatingRowStyle BackColor="#E4E4E4" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Nombre">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbNombre" runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Tipo">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbTipo" runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Tamaño">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbSize" runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Right" />
                                            </asp:TemplateField>
                                            <asp:ButtonField ButtonType="Image" CommandName="DescargarArchivo" HeaderText="Descargar"
                                                ImageUrl="~/Images/Icon/download.png" Text="Descargar" AccessibleHeaderText="btBle">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:ButtonField>
                                        </Columns>
                                        <HeaderStyle BackColor="#666666" ForeColor="White" />
                                        <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <EmptyDataTemplate>
                                            <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                            <asp:Label ID="lbEmptyList" runat="server" Text="  No se han encontrado registros"
                                                Font-Size="14px"></asp:Label>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btDetalleEventoCerrar" runat="server" Text="Cerrar" Width="70px"
                                        CssClass="submitButton" OnClick="btDetalleEventoCerrar_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Detalle Evento !-->
            <!-- Panel Buscar Persona -->
            <asp:HiddenField ID="hfBuscarPersona" runat="server" />
            <asp:ModalPopupExtender ID="hfBuscarPersona_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" PopupControlID="upBuscarPersona" TargetControlID="hfBuscarPersona">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upBuscarPersona" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnBuscarPersona" runat="server" CssClass="Popup_frontground">
                        <table class="tableControls">
                            <tr>
                                <td colspan="2" class="tdTitle">
                                    <h1>
                                        Buscar persona</h1>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label21" runat="server" Text="Apellido"></asp:Label>
                                    <asp:TextBox ID="txtBuscarPersonaApellido" runat="server" AutoPostBack="True"></asp:TextBox>
                                    <asp:Button ID="btBuscarPersonaBuscar" runat="server" Text="Buscar" />
                                    <asp:Button ID="btBuscarPersonaLimpiar" runat="server" Text="Limpiar" OnClick="btBuscarPersonaLimpiar_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:GridView ID="gvBuscarPersonas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                        DataKeyNames="rut" DataSourceID="SDSBuscarPersonas" HorizontalAlign="Center"
                                        OnRowCommand="gvBuscarPersonas_RowCommand" Width="1010px">
                                        <AlternatingRowStyle BackColor="#EDEDED" />
                                        <Columns>
                                            <asp:ButtonField HeaderText="Agregar" ButtonType="Image" ImageUrl="~/Images/Icon/add_user.png"
                                                CommandName="AddEvaluador" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:ButtonField>
                                            <asp:BoundField DataField="rut" HeaderText="RUT" ReadOnly="True" SortExpression="rut" />
                                            <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                                            <asp:BoundField DataField="edad" HeaderText="Edad" ReadOnly="True" SortExpression="edad">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nombre_centro" HeaderText="Centro" SortExpression="nombre_centro">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="cargo" HeaderText="Cargo" SortExpression="cargo">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="antiguedad" HeaderText="Antigüedad" ReadOnly="True" SortExpression="antiguedad">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle BackColor="#666666" ForeColor="White" />
                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                            PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                            NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                                        <PagerStyle HorizontalAlign="Center" />
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="SDSBuscarPersonas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                        SelectCommand="SELECT p.rut, p.nombre, c.nombre AS nombre_centro, DATEDIFF(YEAR, p.fecha_nacimiento, SYSDATETIME()) AS edad, p.cargo, DATEDIFF(YEAR, p.fecha_ingreso, SYSDATETIME()) AS antiguedad FROM Persona p, Centro c WHERE (p.id_centro=c.id) AND (c.id&lt;&gt;@id_ext) AND (c.id&lt;&gt;@id_rsp) AND (p.nombre LIKE '%' + @nombre + '%') ORDER BY p.nombre ASC"
                                        OnSelected="SDSBuscarPersonas_Selected">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="EXT" Name="id_ext" />
                                            <asp:Parameter DefaultValue="RSP" Name="id_rsp" />
                                            <asp:ControlParameter ControlID="txtBuscarPersonaApellido" Name="nombre" PropertyName="Text" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>
                                    <div align="left" style="text-align: center">
                                        <asp:Literal ID="ltBuscarPersonaSummary" runat="server"></asp:Literal>
                                    </div>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btBuscarPersonaCerrar" runat="server" Text="Cerrar" OnClick="btBuscarPersonaCerrar_Click"
                                        CssClass="submitButton" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Buscar Persona !-->
            <!-- Panel Confirmar Acción Inmediata-->
            <asp:HiddenField ID="hfConfirmAccionInmediata" runat="server" />
            <asp:ModalPopupExtender ID="hfConfirmAccionInmediata_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" TargetControlID="hfConfirmAccionInmediata"
                PopupControlID="pnConfirmAccionInmediata">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upConfirmAccionInmediata" runat="server" style="width: 100%"
                UpdateMode="Conditional">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnConfirmAccionInmediata" runat="server" CssClass="Popup_frontground"
                        HorizontalAlign="Center" Width="100%">
                        <asp:Label ID="lbMessageConfirmAccionInmediata" runat="server" CssClass="messageBoxTitle"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btConfirmAccionInmediataSi" runat="server" Text="Si" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmAccionInmediataSi_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btConfirmAccionInmediataNo" runat="server" Text="No" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmAccionInmediataNo_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar Acción Inmediata !-->
            <!-- Panel Message -->
            <asp:HiddenField ID="hfMessage" runat="server" />
            <asp:ModalPopupExtender ID="hfMessage_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" PopupControlID="upMessage" TargetControlID="hfMessage">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upMessage" runat="server" UpdateMode="Always" style="width: 100%">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnMessage" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center">
                        <asp:Label ID="lbMessage" runat="server" CssClass="messageBoxTitle"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btMessageAceptar" runat="server" Text="Aceptar" Width="70px" CssClass="submitButton"
                                        OnClick="btMessageAceptar_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <br />
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Message !-->
            <!-- Panel Confirmar Volver -->
            <asp:HiddenField ID="hfConfirmVolver" runat="server" />
            <asp:ModalPopupExtender ID="hfConfirmVolver_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" TargetControlID="hfConfirmVolver" PopupControlID="pnConfirmVolver">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upConfirmVolver" runat="server" style="width: 100%" UpdateMode="Conditional">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnConfirmVolver" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center"
                        Width="100%">
                        <asp:Label ID="lbConfirmVolver" runat="server" CssClass="messageBoxTitle" Text="Se perderán los cambios no guardados. ¿Desea continuar?"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btConfirmVolverSi" runat="server" Text="Si" Width="70px" CssClass="submitButton"
                                        OnClick="btConfirmVolverSi_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btConfirmVolverNo" runat="server" Text="No" Width="70px" CssClass="submitButton"
                                        OnClick="btConfirmVolverNo_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar Volver !-->
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibAddArchivo" />
            <asp:PostBackTrigger ControlID="gvArchivosEvento" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
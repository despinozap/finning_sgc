<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RegistrarNuevoEvento.aspx.cs" Inherits="NCCSAN.Clientes.RegistrarNuevoEvento" %>

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
    <script type="text/javascript" language="javascript">

        $(document).ready(function () {
            setDatepickerStyle();
        });


        function setDatepickerStyle() {

            $("#<%= txtFecha.ClientID %>").datepicker
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
                $("#<%= txtFecha.ClientID %>").datepicker
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
            Registro Nuevo Evento</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnComponente" runat="server" Visible="False">
                <table style="width: 30%;" align="center" class="tableControls">
                    <tr>
                        <td colspan="2" class="tdTitle">
                            <h1>
                                Panel de Componente</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label18" runat="server" Text="Centro"></asp:Label>
                            &nbsp;<asp:Label ID="Label28" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlCentro" runat="server" Width="200px" DataSourceID="SDSCentro"
                                DataTextField="nombre" DataValueField="id" AutoPostBack="True" 
                                OnDataBound="ddlCentro_DataBound" >
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSCentro" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT c.id, c.nombre FROM CentroCliente cc, Centro c WHERE (cc.id_centro=c.id) AND (cc.nombre_cliente=@nombre_cliente)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="nombre_cliente" SessionField="nombre_cliente" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" width="30%">
                            <asp:Label ID="Label8" runat="server" Text="W/O"></asp:Label>
                        </td>
                        <td class="tdUnique" width="70%">
                            <asp:TextBox ID="txtWO" runat="server" Width="200px" AutoPostBack="True" OnTextChanged="txtWO_TextChanged"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label1" runat="server" Text="Tipo"></asp:Label>
                            &nbsp;<asp:Label ID="Label29" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlTipo" runat="server" AutoPostBack="True" DataSourceID="SDSTipo"
                                DataTextField="nombre" DataValueField="nombre" OnDataBound="ddlTipo_DataBound"
                                OnSelectedIndexChanged="ddlTipo_SelectedIndexChanged" Width="200px">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSTipo" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [nombre] FROM [TipoEquipo] ORDER BY nombre"></asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label2" runat="server" Text="Modelo"></asp:Label>
                            &nbsp;<asp:Label ID="Label30" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlModelo" runat="server" DataSourceID="SDSModelo" DataTextField="modelo_equipo"
                                DataValueField="modelo_equipo" Style="margin-left: 0px" Width="200px" OnDataBound="ddlModelo_DataBound"
                                AutoPostBack="True">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSModelo" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [modelo_equipo] FROM [EquipoTipoEquipo] WHERE ([nombre_tipoequipo] = @tipo) ORDER BY modelo_equipo">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlTipo" Name="tipo" PropertyName="SelectedValue"
                                        Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label26" runat="server" Text="Serie equipo"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtSerieEquipo" runat="server" Width="200px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label3" runat="server" Text="Sistema"></asp:Label>
                            &nbsp;<asp:Label ID="Label31" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlSistema" runat="server" AutoPostBack="True" DataSourceID="SDSSistema"
                                DataTextField="nombre_sistema" DataValueField="nombre_sistema" OnDataBound="ddlSistema_DataBound"
                                Width="200px" OnSelectedIndexChanged="ddlSistema_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSSistema" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [nombre_sistema] FROM [TipoEquipoSistema] WHERE ([nombre_tipoequipo] = @tipo) ORDER BY nombre_sistema">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlTipo" Name="tipo" PropertyName="SelectedValue"
                                        Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label5" runat="server" Text="Sub-sistema"></asp:Label>
                            &nbsp;<asp:Label ID="Label32" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlSubsistema" runat="server" AutoPostBack="True" DataSourceID="SDSSubsistema"
                                DataTextField="nombre_subsistema" DataValueField="nombre_subsistema" OnDataBound="ddlSubsistema_DataBound"
                                Width="200px" OnSelectedIndexChanged="ddlSubsistema_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSSubsistema" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [nombre_subsistema] FROM [TipoEquipoSistemaSubsistema] WHERE (([nombre_tipoequipo] = @tipo) AND ([nombre_sistema] = @nombre_sistema)) ORDER BY nombre_subsistema">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlTipo" Name="tipo" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ddlSistema" Name="nombre_sistema" PropertyName="SelectedValue"
                                        Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label4" runat="server" Text="Componente"></asp:Label>
                            &nbsp;<asp:Label ID="Label33" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlComponente" runat="server" DataSourceID="SDSComponente"
                                DataTextField="nombre_componente" DataValueField="nombre_componente" Width="200px"
                                OnDataBound="ddlComponente_DataBound">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSComponente" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [nombre_componente] FROM [TipoEquipoComponente] WHERE (([nombre_tipoequipo] = @tipo) AND ([nombre_sistema] = @nombre_sistema) AND ([nombre_subsistema] = @nombre_subsistema))">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlTipo" Name="tipo" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ddlSistema" Name="nombre_sistema" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ddlSubsistema" Name="nombre_subsistema" PropertyName="SelectedValue"
                                        Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <asp:HiddenField ID="hfNombreComponente" runat="server" ViewStateMode="Enabled" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label7" runat="server" Text="Serie componente"></asp:Label>
                            &nbsp;<asp:Label ID="Label10" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtSerieComponente" runat="server" Width="200px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center" style="width: 97px">
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:ImageButton ID="ibNextComponente" runat="server" ImageUrl="~/Images/Button/next.png"
                                OnClick="ibNextComponente_Click" CssClass="submitPanelButton" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnDesviacion" runat="server" Visible="False">
                <table align="center" class="tableControls" width="50%">
                    <tr>
                        <td colspan="2" class="tdTitle">
                            <h1>
                                Panel de Desviación</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" width="30%">
                            <asp:Label ID="Label19" runat="server" Text="Parte o Pieza"></asp:Label>
                        </td>
                        <td class="tdUnique" width="70%">
                            <asp:TextBox ID="txtParte" runat="server" Width="190px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label27" runat="server" Text="Número parte"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtNumeroParte" runat="server" Width="190px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label22" runat="server" Text="Horas de trabajo"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtHoras" runat="server" Width="190px" TextMode="Number"></asp:TextBox>
                            <asp:Label ID="Label25" runat="server" Text="(Sólo si aplica)"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label9" runat="server" Text="Fecha identificación de falla"></asp:Label>
                            &nbsp;<asp:Label ID="Label34" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtFecha" runat="server" Width="190px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label21" runat="server" Text="Clasificación"></asp:Label>
                            &nbsp;<asp:Label ID="Label38" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlClasificacion" runat="server" AutoPostBack="True" DataSourceID="SDSClasificacion"
                                DataTextField="nombre_clasificacion" DataValueField="nombre_clasificacion" OnDataBound="ddlClasificacion_DataBound"
                                Width="300px">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSClasificacion" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT nombre_clasificacion FROM CentroClasificacionSubclasificacion WHERE (id_centro=@id_centro)">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlCentro" Name="id_centro" PropertyName="SelectedValue" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label13" runat="server" Text="Sub-clasificación"></asp:Label>
                            &nbsp;<asp:Label ID="Label39" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlSubclasificacion" runat="server" OnDataBound="ddlSubclasificacion_DataBound"
                                Width="300px" DataSourceID="SDSSubclasificacion" DataTextField="nombre_subclasificacion"
                                DataValueField="nombre_subclasificacion">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSSubclasificacion" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [nombre_subclasificacion] FROM [CentroClasificacionSubclasificacion] WHERE ([nombre_clasificacion] = @nombre_clasificacion) AND (id_centro=@id_centro)">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlClasificacion" Name="nombre_clasificacion" PropertyName="SelectedValue" />
                                    <asp:ControlParameter ControlID="ddlCentro" Name="id_centro" PropertyName="SelectedValue" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label14" runat="server" Text="Detalle"></asp:Label>
                            &nbsp;<asp:Label ID="Label40" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtDetalle" runat="server" Height="113px" MaxLength="3000" TextMode="MultiLine"
                                Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center" style="width: 140px">
                    <tr>
                        <td>
                            <asp:ImageButton ID="ibPreviousDesviacion" runat="server" ImageUrl="~/Images/Button/back.png"
                                OnClick="ibPreviousDesviacion_Click" CssClass="submitPanelButton" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ibNextDesviacion" runat="server" ImageUrl="~/Images/Button/next.png"
                                OnClick="ibNextDesviacion_Click" CssClass="submitPanelButton" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnArchivos" runat="server" Visible="False" HorizontalAlign="Center">
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
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td>
                            <asp:ImageButton ID="ibPreviousArchivos" runat="server" OnClick="ibPreviousArchivos_Click"
                                CssClass="submitPanelButton" ImageUrl="~/Images/Button/back.png" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ibRegisterEvento" runat="server" CssClass="submitPanelButton"
                                ImageUrl="~/Images/Button/save.png" OnClick="ibRegisterEvento_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div align="left" style="color: #FF0000; text-align: center">
                <asp:Literal ID="ltSummary" runat="server"></asp:Literal>
            </div>
            <!-- Panel Load WorkOrder -->
            <asp:HiddenField ID="hfLoadWO" runat="server" />
            <asp:ModalPopupExtender ID="hfLoadWO_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" PopupControlID="upLoadWO" TargetControlID="hfLoadWO">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upLoadWO" runat="server" UpdateMode="Always" style="width: 100%">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnLoadWO" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center">
                        <asp:Label ID="Label23" runat="server" CssClass="messageBoxTitle"></asp:Label>
                        <br />
                        <asp:Label ID="lbMessageLoadWO" runat="server" CssClass="messageBoxMessage"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btLoadWOSi" runat="server" Text="Si" Width="70px" CssClass="submitButton"
                                        OnClick="btLoadWOSi_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btLoadWONo" runat="server" Text="No" Width="70px" CssClass="submitButton"
                                        OnClick="btLoadWONo_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <br />
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btLoadWOSi" />
                </Triggers>
            </asp:UpdatePanel>
            <!-- Panel Load WorkOrder !-->
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
                        <asp:Label ID="Label6" runat="server" Text="¿Desea ingresar otro Evento para el mismo Componente?"
                            CssClass="messageBoxMessage"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btMessageSi" runat="server" Text="Si" OnClick="btMessageSi_Click"
                                        Width="70px" CssClass="submitButton" />
                                </td>
                                <td>
                                    <asp:Button ID="btMessageNo" runat="server" Text="No" OnClick="btMessageNo_Click"
                                        Width="70px" CssClass="submitButton" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <br />
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btMessageSi" />
                </Triggers>
            </asp:UpdatePanel>
            <!-- Panel Message !-->
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibAddArchivo" />
            <asp:PostBackTrigger ControlID="ibNextDesviacion" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

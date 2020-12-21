<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RegistrarNuevoEvento.aspx.cs" Inherits="NCCSAN.RegistrarNuevoEvento"
    EnableEventValidation="false" %>

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
                        <td class="tdUnique" width="30%">
                            <asp:Label ID="Label8" runat="server" Text="W/O"></asp:Label>
                            &nbsp;<asp:Label ID="Label42" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique" width="70%">
                            <asp:TextBox ID="txtWO" runat="server" Width="200px" AutoPostBack="True" OnTextChanged="txtWO_TextChanged"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label18" runat="server" Text="Cliente"></asp:Label>
                            &nbsp;<asp:Label ID="Label28" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlCliente" runat="server" DataSourceID="SDSCliente" DataTextField="nombre_cliente"
                                DataValueField="nombre_cliente" OnDataBound="ddlCliente_DataBound" Width="200px">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSCliente" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [nombre_cliente] FROM [CentroCliente] WHERE ([id_centro] = @id_centro)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
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
                            &nbsp;<asp:Label ID="lbAsteriskSerieEquipo" runat="server" Text="*" ForeColor="Red"
                                Font-Bold="True"></asp:Label>
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
                            &nbsp;</td>
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
                            <asp:TextBox ID="txtFecha" runat="server" Width="190px" OnTextChanged="txtFecha_TextChanged"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label10" runat="server" Text="Fuente"></asp:Label>
                            &nbsp;<asp:Label ID="Label35" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlFuente" runat="server" DataSourceID="SDSFuente" DataTextField="nombre"
                                DataValueField="nombre" OnDataBound="ddlFuente_DataBound" Width="300px" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlFuente_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSFuente" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [nombre] FROM [Fuente]"></asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label11" runat="server" Text="Área"></asp:Label>
                            &nbsp;<asp:Label ID="Label36" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlArea" runat="server" AutoPostBack="True" DataSourceID="SDSArea"
                                DataTextField="nombre_area" DataValueField="nombre_area" OnDataBound="ddlArea_DataBound"
                                Width="300px" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSArea" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [nombre_area] FROM [CentroArea] WHERE ([id_centro] = @id_centro)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label12" runat="server" Text="Sub-área"></asp:Label>
                            &nbsp;<asp:Label ID="Label37" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlSubarea" runat="server" DataSourceID="SDSSubarea" DataTextField="nombre_subarea"
                                DataValueField="nombre_subarea" OnDataBound="ddlSubarea_DataBound" Width="300px"
                                AutoPostBack="True" OnSelectedIndexChanged="ddlSubarea_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSSubarea" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT DISTINCT [nombre_subarea] FROM [CentroAreaSubarea] WHERE (([id_centro] = @id_centro) AND ([nombre_area] = @nombre_area))">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" Type="String" />
                                    <asp:ControlParameter ControlID="ddlArea" Name="nombre_area" PropertyName="SelectedValue"
                                        Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
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
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
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
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label67" runat="server" Text="Agente corrector"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtAgenteCorrector" runat="server" Width="190px"></asp:TextBox>
                            <asp:Label ID="Label68" runat="server" Text=" (Sólo CSAR)"></asp:Label>
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
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label15" runat="server" Text="Probabilidad"></asp:Label>
                            &nbsp;<asp:Label ID="Label41" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlProbabilidad" runat="server" Width="200px" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlProbabilidad_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label16" runat="server" Text="Consecuencia"></asp:Label>
                            &nbsp;<asp:Label ID="Label43" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlConsecuencia" runat="server" Width="200px" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlConsecuencia_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:ImageButton ID="ibMatrizConsecuencia" runat="server" ImageUrl="~/Images/Icon/help.png"
                                OnClick="ibMatrizConsecuencia_Click" ImageAlign="Middle" ToolTip="Ver matriz de consecuencia" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label17" runat="server" Text="IRC"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="lbIRC" runat="server" Text="--"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label20" runat="server" Text="Criticidad"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="lbCriticidad" runat="server" Text="--"></asp:Label>
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
                    <tr>
                        <td colspan="2">
                            <asp:ImageButton ID="ibByPassDesviacion" runat="server" ImageUrl="~/Images/Button/bypass.png"
                                OnClick="ibByPassDesviacion_Click" Visible="False" CssClass="submitPanelButton" />
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
                        <td colspan="2">
                            <asp:CheckBox ID="chbAccionInmediata" runat="server" Text="Registrar Acción Inmediata junto al Evento"
                                AutoPostBack="True" OnCheckedChanged="chbAccionInmediata_CheckedChanged" Visible="False" />
                            <br />
                            <br />
                        </td>
                    </tr>
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
            <asp:Panel ID="pnEventoAccionInmediata" runat="server" Visible="False">
                <asp:Panel ID="pnCausas" runat="server" Visible="True">
                    <table align="center" class="tableControls">
                        <tr>
                            <td colspan="2" class="tdTitle">
                                <h1>
                                    Panel de Causas</h1>
                            </td>
                        </tr>
                        <tr>
                            <td class="tdUnique" width="30%">
                                <asp:Label ID="Label65" runat="server" Text="Origen de falla" Font-Bold="False"></asp:Label>
                                &nbsp;<asp:Label ID="Label66" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
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
                            <td class="tdUnique">
                                <asp:Label ID="Label63" runat="server" Text="Tipo causa inmediata" Font-Bold="False"></asp:Label>
                                &nbsp;<asp:Label ID="Label64" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                            </td>
                            <td class="tdUnique" align="left">
                                <asp:DropDownList ID="ddlTipoCausaInmediata" runat="server" Width="230px" 
                                    AutoPostBack="True" DataTextField="tipo" 
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
                                <asp:Label ID="Label24" runat="server" Text="Causa inmediata"></asp:Label>
                                &nbsp;<asp:Label ID="Label44" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
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
                                <asp:Label ID="Label45" runat="server" Text="Sub-causa inmediata"></asp:Label>
                                &nbsp;<asp:Label ID="Label46" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
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
                                <asp:Label ID="Label61" runat="server" Text="Tipo causa básica" Font-Bold="False"></asp:Label>
                                &nbsp;<asp:Label ID="Label62" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
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
                                <asp:Label ID="Label47" runat="server" Text="Causa básica" Font-Bold="False"></asp:Label>
                                &nbsp;<asp:Label ID="Label48" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                            </td>
                            <td class="tdUnique" align="left">
                                <asp:DropDownList ID="ddlCausaBasica" runat="server" AutoPostBack="True" DataTextField="nombre"
                                    DataValueField="nombre" OnDataBound="ddlCausaBasica_DataBound" OnSelectedIndexChanged="ddlCausaBasica_SelectedIndexChanged"
                                    Width="230px" DataSourceID="SDSCausaBasica">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SDSCausaBasica" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                    SelectCommand="SELECT [nombre] FROM [CausaBasica] WHERE ([tipo]=@tipo)">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlTipoCausaBasica" Name="tipo" 
                                            PropertyName="SelectedValue" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td class="tdUnique" align="left">
                                <asp:Label ID="Label49" runat="server" Text="Sub-causa básica"></asp:Label>
                                &nbsp;<asp:Label ID="Label50" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
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
                                <asp:Label ID="Label59" runat="server" Text="Responsable del Evento"></asp:Label>
                                &nbsp;<asp:Label ID="Label60" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                            </td>
                            <td class="tdUnique" align="left">
                                <asp:DropDownList ID="ddlResponsableEvento" runat="server" Width="230px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
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
                                <asp:Label ID="Label51" runat="server" Text="Acción inmediata"></asp:Label>
                                &nbsp;<asp:Label ID="Label52" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                            </td>
                            <td class="tdUnique" width="80%">
                                <asp:TextBox ID="txtAccionInmediata" runat="server" Width="100%" Height="113px" MaxLength="3000"
                                    TextMode="MultiLine"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tdUnique">
                                <asp:Label ID="Label53" runat="server" Text="Fecha"></asp:Label>
                                &nbsp;<asp:Label ID="Label54" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                            </td>
                            <td class="tdUnique">
                                <asp:TextBox ID="txtFechaAccionInmediata" runat="server" Width="190px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tdUnique">
                                <asp:Label ID="Label55" runat="server" Text="Efectividad"></asp:Label>
                                &nbsp;<asp:Label ID="Label56" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
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
                                <asp:Label ID="Label57" runat="server" Text="Observación"></asp:Label>
                            </td>
                            <td class="tdUnique">
                                <asp:TextBox ID="txtAccionInmediataObservacion" runat="server" Height="113px" MaxLength="3000"
                                    TextMode="MultiLine" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                </asp:Panel>
                <br />
                <br />
                <asp:Panel ID="pnArchivosAccionInmediata" runat="server">
                    <table align="center" class="tableControls">
                        <tr>
                            <td colspan="2" class="tdTitle">
                                <h1>
                                    Panel de Archivos Accion Inmediata</h1>
                            </td>
                        </tr>
                        <tr>
                            <td class="tdFirst">
                                <asp:GridView ID="gvArchivosAccionInmediata" runat="server" AutoGenerateColumns="False"
                                    Width="963px" OnRowCommand="gvArchivosAccionInmediata_RowCommand" HorizontalAlign="Center"
                                    ShowHeaderWhenEmpty="True">
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
                                <asp:FileUpload ID="fuArchivoAccionInmediata" runat="server" Width="500" multiple="true" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tdLast" align="center">
                                <asp:ImageButton ID="ibAddArchivoAccionInmediata" runat="server" ImageAlign="AbsMiddle"
                                    ImageUrl="~/Images/Button/attachment.png" OnClick="ibAddArchivoAccionInmediata_Click"
                                    CssClass="submitAttachFile" />
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
                <table align="center">
                    <tr>
                        <td>
                            <asp:ImageButton ID="ibPreviousAccionInmediata" runat="server" CssClass="submitPanelButton"
                                ImageUrl="~/Images/Button/back.png" OnClick="ibPreviousAccionInmediata_Click" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ibRegistrarEventoAccionInmediata" runat="server" CssClass="submitPanelButton"
                                ImageUrl="~/Images/Button/save.png" OnClick="ibRegistrarEventoAccionInmediata_Click" />
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
            <!-- Panel Message Componente -->
            <asp:HiddenField ID="hfMessageComponente" runat="server" />
            <asp:ModalPopupExtender ID="hfMessageComponente_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" PopupControlID="upMessageComponente" TargetControlID="hfMessageComponente">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upMessageComponente" runat="server" UpdateMode="Always" style="width: 100%">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnMessageComponente" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center">
                        <asp:Label ID="lbMessageComponente" runat="server" CssClass="messageBoxTitle"></asp:Label>
                        <br />
                        <asp:Label ID="Label6" runat="server" Text="¿Desea ingresar otro Evento para el mismo Componente?"
                            CssClass="messageBoxMessage"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btMessageComponenteSi" runat="server" Text="Si" OnClick="btMessageComponenteSi_Click"
                                        Width="70px" CssClass="submitButton" />
                                </td>
                                <td>
                                    <asp:Button ID="btMessageComponenteNo" runat="server" Text="No" OnClick="btMessageComponenteNo_Click"
                                        Width="70px" CssClass="submitButton" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <br />
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btMessageComponenteSi" />
                </Triggers>
            </asp:UpdatePanel>
            <!-- Panel Message Componente !-->
            <!-- Panel Message Equipo -->
            <asp:HiddenField ID="hfMessageEquipo" runat="server" />
            <asp:ModalPopupExtender ID="hfMessageEquipo_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" PopupControlID="upMessageEquipo" TargetControlID="hfMessageEquipo">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upMessageEquipo" runat="server" UpdateMode="Always" style="width: 100%">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnMessageEquipo" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center">
                        <asp:Label ID="lbMessageEquipo" runat="server" CssClass="messageBoxTitle"></asp:Label>
                        <br />
                        <asp:Label ID="Label76" runat="server" Text="¿Desea ingresar otro Evento para el mismo Equipo?"
                            CssClass="messageBoxMessage"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btMessageEquipoSi" runat="server" Text="Si" OnClick="btMessageEquipoSi_Click"
                                        Width="70px" CssClass="submitButton" />
                                </td>
                                <td>
                                    <asp:Button ID="btMessageEquipoNo" runat="server" Text="No" OnClick="btMessageEquipoNo_Click"
                                        Width="70px" CssClass="submitButton" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <br />
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btMessageEquipoSi" />
                </Triggers>
            </asp:UpdatePanel>
            <!-- Panel Message Equipo !-->
            <!-- Panel Matriz Consecuencia -->
            <asp:HiddenField ID="hfMatrizConsecuencia" runat="server" />
            <asp:ModalPopupExtender ID="hfMatrizConsecuencia_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" PopupControlID="pnMatrizConsecuencia"
                TargetControlID="hfMatrizConsecuencia">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upMatrizConsecuencia" UpdateMode="Always" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="pnMatrizConsecuencia" runat="server" HorizontalAlign="Center" CssClass="Popup_frontground">
                        <asp:Image ID="imgMatrizConsecuencia" runat="server" />
                        <br />
                        <asp:Button ID="btCerrarMatrizConsecuencia" runat="server" Text="Cerrar" OnClick="btCerrarMatrizConsecuencia_Click"
                            CssClass="submitButton" />
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Matriz Consecuencia !-->
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
                                    <asp:Label ID="Label58" runat="server" Text="Apellido"></asp:Label>
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
                                        SelectCommand="SELECT p.rut, p.nombre, c.nombre AS nombre_centro, DATEDIFF(YEAR, p.fecha_nacimiento, SYSDATETIME()) AS edad, p.cargo, DATEDIFF(YEAR, p.fecha_ingreso, SYSDATETIME()) AS antiguedad FROM Persona p, Centro c WHERE (p.id_centro=c.id) AND (p.nombre_clasificacionpersona&lt;&gt;'Clientes') AND (p.nombre LIKE '%' + @nombre + '%') ORDER BY p.nombre ASC"
                                        OnSelected="SDSBuscarPersonas_Selected">
                                        <SelectParameters>
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
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibAddArchivo" />
            <asp:PostBackTrigger ControlID="ibAddArchivoAccionInmediata" />
            <asp:PostBackTrigger ControlID="ibNextDesviacion" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

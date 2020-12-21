<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EventosRegistrados.aspx.cs" Inherits="NCCSAN.Personas.EventosRegistrados" %>

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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Mis eventos registrados</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="hfRutPersona" runat="server" />
            <asp:HiddenField ID="hfCodigoEventoEliminar" runat="server" />
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls" style="width: 100%">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label3" runat="server" Text="Código o W/O"></asp:Label>
                            <asp:TextBox ID="txtCodigoWO" runat="server" Width="120px"></asp:TextBox>
                            <asp:ImageButton ID="ibSearch" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Icon/search.png"
                                AlternateText="Buscar" ToolTip="Buscar" />
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label1" runat="server" Text="Cliente"></asp:Label>
                            <asp:DropDownList ID="ddlCliente" runat="server" DataSourceID="SDSCliente" DataTextField="nombre_cliente"
                                DataValueField="nombre_cliente" OnDataBound="ddlCliente_DataBound" AutoPostBack="True"
                                Width="200">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSCliente" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT [nombre_cliente] FROM [CentroCliente] WHERE ([id_centro] = @id_centro)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label5" runat="server" Text="Fuente"></asp:Label>
                            <asp:DropDownList ID="ddlFuente" runat="server" AutoPostBack="True"
                                Width="200px" DataSourceID="SDSFuente" DataTextField="nombre" 
                                DataValueField="nombre" ondatabound="ddlFuente_DataBound">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSFuente" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" 
                                SelectCommand="SELECT DISTINCT [nombre] FROM [Fuente]"></asp:SqlDataSource>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label6" runat="server" Text="Área"></asp:Label>
                            <asp:DropDownList ID="ddlArea" runat="server" AutoPostBack="True"
                                Width="200px" DataSourceID="SDSArea" DataTextField="nombre_area" 
                                DataValueField="nombre_area" ondatabound="ddlArea_DataBound">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSArea" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" 
                                SelectCommand="SELECT [nombre_area] FROM [CentroArea] WHERE ([id_centro] = @id_centro)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="100px" AutoPostBack="True" OnSelectedIndexChanged="ddlAnio_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label2" runat="server" Text="Mes"></asp:Label>
                            <asp:DropDownList ID="ddlMes" runat="server" Width="120px" AutoPostBack="True" OnSelectedIndexChanged="ddlMes_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:HiddenField ID="hfFecha" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" colspan="5" align="center">
                            <asp:HiddenField ID="hfFiltroEstado" runat="server" />
                            <asp:Label ID="Label25" runat="server" Text="Estado" Font-Bold="True"></asp:Label>
                            <asp:CheckBox ID="chbFiltroEstado" runat="server" AutoPostBack="True" Checked="True"
                                OnCheckedChanged="chbFiltroEstado_CheckedChanged" Text="Todos los Eventos" />
                            &nbsp;&nbsp;&nbsp;
                            <asp:DropDownList ID="ddlEstado1" runat="server" AutoPostBack="True" Enabled="False">
                            </asp:DropDownList>
                            <asp:Label ID="Label28" runat="server" Text=" (y) "></asp:Label>
                            <asp:DropDownList ID="ddlEstado2" runat="server" AutoPostBack="True" Enabled="False">
                            </asp:DropDownList>
                            <asp:Label ID="Label29" runat="server" Text=" (y) "></asp:Label>
                            <asp:DropDownList ID="ddlEstado3" runat="server" AutoPostBack="True" Enabled="False">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table class="tableControls" align="center" style="width: 100%">
                <tr>
                    <td class="tdUnique">
                        <asp:GridView ID="gvEventos" runat="server" AutoGenerateColumns="False" DataKeyNames="codigo"
                            DataSourceID="SDSEventos" HorizontalAlign="Center" Width="100%" AllowPaging="True"
                            OnDataBound="gvEventos_DataBound" PageSize="20" OnRowCommand="gvEventos_RowCommand"
                            ShowHeaderWhenEmpty="True">
                            <AlternatingRowStyle CssClass="rowDataAlt" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Image ID="imgEstadoIcono" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="codigo" HeaderText="Código" ReadOnly="True" SortExpression="codigo">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="work_order" HeaderText="W/O" SortExpression="work_order">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="fecha_ingreso" HeaderText="Fecha" SortExpression="fecha_ingreso">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_cliente" HeaderText="Cliente" SortExpression="nombre_cliente">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_fuente" HeaderText="Fuente" SortExpression="nombre_fuente">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="irc" HeaderText="IRC" SortExpression="irc">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_componente" HeaderText="Componente" SortExpression="nombre_componente">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Días activo">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="lbDiasActivo" runat="server" Text='<%#Eval("dias_actividad") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="estado" HeaderText="Estado" SortExpression="estado">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:ButtonField ButtonType="Image" CommandName="DetalleEvento" HeaderText="Detalle"
                                    ImageUrl="~/Images/Icon/search.png" Text="Detalle">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Image" CommandName="EditarEvento" HeaderText="Editar"
                                    ImageUrl="~/Images/Icon/edit.png" Text="Editar">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Image" CommandName="EliminarEvento" 
                                    HeaderText="Eliminar" ImageUrl="~/Images/Icon/remove_file.png" Text="Eliminar">
                                <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                            </Columns>
                            <HeaderStyle BackColor="#666666" ForeColor="White" />
                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                            <PagerStyle HorizontalAlign="Center" />
                            <RowStyle CssClass="rowData" />
                            <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            <EmptyDataTemplate>
                                <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                <asp:Label ID="lbEmptyList" runat="server" Text="  No se han encontrado registros"
                                    Font-Size="14px"></asp:Label>
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SDSEventos" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                            
                            
                            
                            
                            SelectCommand="SELECT e.codigo, e.work_order, CONVERT(VARCHAR(10), e.fecha_ingreso, 105) AS fecha_ingreso, e.nombre_cliente, e.nombre_fuente, e.irc, e.nombre_componente, DATEDIFF(DAY, e.fecha_ingreso, SYSDATETIME()) as dias_actividad, e.estado FROM Evento e, EventoPersona ep WHERE (e.id_centro=@id_centro) AND (ep.codigo_evento=e.codigo) AND (ep.rut_persona=@rut_persona) AND (ep.tipo=@tipo_persona) AND (e.nombre_cliente LIKE @nombre_cliente + '%') AND (e.nombre_area LIKE @nombre_area + '%') AND (e.nombre_fuente LIKE @nombre_fuente + '%') AND (CONVERT(VARCHAR(10), e.fecha_ingreso, 105) LIKE '%' + @fecha + '%') AND ((e.codigo LIKE '%' + @codigowo + '%') OR  (e.work_order LIKE '%' + @codigowo + '%')) AND ((e.estado LIKE '%' + @filtroEstado + '%') OR (e.estado IN (@estado1, @estado2, @estado3))) ORDER BY e.fecha_ingreso DESC">
                            <SelectParameters>
                                <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                <asp:ControlParameter ControlID="hfRutPersona" Name="rut_persona" 
                                    PropertyName="Value" />
                                <asp:Parameter DefaultValue="Creador" Name="tipo_persona" />
                                <asp:ControlParameter ControlID="ddlCliente" DefaultValue="" Name="nombre_cliente"
                                    PropertyName="SelectedValue" ConvertEmptyStringToNull="False" Type="String" />
                                <asp:ControlParameter ControlID="ddlArea" ConvertEmptyStringToNull="False" 
                                    Name="nombre_area" PropertyName="SelectedValue" />
                                <asp:ControlParameter ControlID="ddlFuente" ConvertEmptyStringToNull="False" 
                                    Name="nombre_fuente" PropertyName="SelectedValue" />
                                <asp:ControlParameter ControlID="hfFecha" DefaultValue="" Name="fecha" PropertyName="Value"
                                    ConvertEmptyStringToNull="False" Type="String" />
                                <asp:ControlParameter ControlID="txtCodigoWO" ConvertEmptyStringToNull="False" Name="codigowo"
                                    PropertyName="Text" Type="String" />
                                <asp:ControlParameter ControlID="hfFiltroEstado" ConvertEmptyStringToNull="False"
                                    Name="filtroEstado" PropertyName="Value" Type="String" />
                                <asp:ControlParameter ControlID="ddlEstado1" ConvertEmptyStringToNull="False" Name="estado1"
                                    PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="ddlEstado2" ConvertEmptyStringToNull="False" Name="estado2"
                                    PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="ddlEstado3" ConvertEmptyStringToNull="False" Name="estado3"
                                    PropertyName="SelectedValue" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td class="tdUnique" align="left">
                        <asp:Panel ID="pnOpcionesLista" runat="server" Visible="False">
                            <asp:ImageButton ID="ibExportToExcel" runat="server" ImageUrl="~/Images/Icon/ms_excel.png"
                                OnClick="ibExportToExcel_Click" ToolTip="Descargar como planilla Excel" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <!-- Panel Confirmar eliminar Evento-->
            <asp:HiddenField ID="hfConfirmEliminarEvento" runat="server" />
            <asp:ModalPopupExtender ID="hfConfirmEliminarEvento_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" TargetControlID="hfConfirmEliminarEvento"
                PopupControlID="pnConfirmEliminarEvento">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upConfirmEliminarEvento" runat="server" style="width: 100%"
                UpdateMode="Conditional">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnConfirmEliminarEvento" runat="server" CssClass="Popup_frontground"
                        HorizontalAlign="Center" Width="100%">
                        <asp:Label ID="lbMessageConfirmEliminarEvento" runat="server" CssClass="messageBoxTitle"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btConfirmEliminarEventoSi" runat="server" Text="Si" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmEliminarEventoSi_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btConfirmEliminarEventoNo" runat="server" Text="No" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmEliminarEventoNo_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar eliminar Evento !-->
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
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListarAccionesCorrectivas.aspx.cs" Inherits="NCCSAN.PlanesAccion.ListarAccionesCorrectivas" %>

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
            Lista de Acciones Correctivas</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls" style="width: 100%">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label3" runat="server" Text="RUT Responsable"></asp:Label>
                            <asp:TextBox ID="txtBuscarRUTResponsable" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="ibSearchRUTResponsable" runat="server" ImageAlign="AbsMiddle"
                                ImageUrl="~/Images/Icon/search.png" ToolTip="Buscar" />
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label1" runat="server" Text="Nombre Responsable"></asp:Label>
                            <asp:TextBox ID="txtBuscarNombreResponsable" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="ibSearchNombreResponsable" runat="server" ImageAlign="AbsMiddle"
                                ImageUrl="~/Images/Icon/search.png" ToolTip="Buscar" />
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label10" runat="server" Text="Área"></asp:Label>
                            <asp:DropDownList ID="ddlArea" runat="server" Width="150px" AutoPostBack="True" 
                                DataSourceID="SDSArea" DataTextField="nombre_area" DataValueField="nombre_area" 
                                ondatabound="ddlArea_DataBound">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSArea" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" 
                                SelectCommand="SELECT DISTINCT [nombre_area] FROM [CentroArea] WHERE ([id_centro] = @id_centro)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px" AutoPostBack="True" OnSelectedIndexChanged="ddlAnio_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label2" runat="server" Text="Mes"></asp:Label>
                            <asp:DropDownList ID="ddlMes" runat="server" Width="150px" AutoPostBack="True" OnSelectedIndexChanged="ddlMes_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:HiddenField ID="hfFecha" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table class="tableControls" align="center" style="width: 100%">
                <tr>
                    <td class="tdUnique">
                        <asp:GridView ID="gvAccionesCorrectivas" runat="server" AutoGenerateColumns="False"
                            DataSourceID="SDSAccionesCorrectivas" HorizontalAlign="Center" Width="100%" AllowPaging="True"
                            PageSize="20" OnRowCommand="gvAccionesCorrectivas_RowCommand" ShowHeaderWhenEmpty="True"
                            OnDataBound="gvAccionesCorrectivas_DataBound">
                            <AlternatingRowStyle CssClass="rowDataAlt" />
                            <Columns>
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Image ID="imgEstadoIcono" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="codigo_planaccion" HeaderText="Plan de Acción" SortExpression="codigo_planaccion" />
                                <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion" />
                                <asp:BoundField DataField="fecha_limite" HeaderText="Fecha límite" SortExpression="fecha_limite">
                                    <HeaderStyle Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="fecha_realizado" HeaderText="Fecha ejecución" SortExpression="fecha_realizado"
                                    ConvertEmptyStringToNull="False">
                                    <HeaderStyle Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="rut_responsable" HeaderText="RUT responsable" SortExpression="rut_responsable" />
                                <asp:BoundField DataField="nombre_responsable" HeaderText="Nombre responsable" SortExpression="nombre_responsable">
                                    <HeaderStyle Width="230px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Detalle">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibDetalleAccionCorrectiva" runat="server" ImageAlign="Middle"
                                            ImageUrl="~/Images/Icon/search.png" CommandName="DetalleAccionCorrectiva" CommandArgument='<%#Eval("id")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
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
                        <asp:SqlDataSource ID="SDSAccionesCorrectivas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                            
                            
                            
                            SelectCommand="SELECT ac.id, ac.codigo_planaccion, ac.descripcion, CONVERT(VARCHAR(10), ac.fecha_limite, 105) AS fecha_limite, CONVERT(VARCHAR(10), ac.fecha_realizado, 105) AS fecha_realizado, ac.rut_responsable, p.nombre AS nombre_responsable FROM AccionCorrectiva ac, Persona p, Evento e WHERE (p.rut=ac.rut_responsable) AND (ac.codigo_planaccion=e.codigo) AND (e.id_centro=@id_centro) AND (e.nombre_area LIKE @nombre_area + '%') AND ((p.rut LIKE '%' + @rut_responsable + '%') AND (p.nombre LIKE '%' + @nombre_responsable + '%') AND (CONVERT(VARCHAR(10), ac.fecha_limite, 105) LIKE '%' + @fecha + '%')) ORDER BY ac.fecha_limite DESC">
                            <SelectParameters>
                                <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                <asp:ControlParameter ControlID="ddlArea" ConvertEmptyStringToNull="False" 
                                    Name="nombre_area" PropertyName="SelectedValue" />
                                <asp:ControlParameter ControlID="txtBuscarRUTResponsable" ConvertEmptyStringToNull="False"
                                    Name="rut_responsable" PropertyName="Text" Type="String" />
                                <asp:ControlParameter ControlID="txtBuscarNombreResponsable" ConvertEmptyStringToNull="False"
                                    Name="nombre_responsable" PropertyName="Text" Type="String" />
                                <asp:ControlParameter ControlID="hfFecha" ConvertEmptyStringToNull="False" Name="fecha"
                                    PropertyName="Value" Type="String" />
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
            <!-- Detalle Acción Correctiva -->
            <asp:HiddenField ID="hfDetalleAccionCorrectiva" runat="server" />
            <asp:ModalPopupExtender ID="hfDetalleAccionCorrectiva_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" PopupControlID="upDetalleAccionCorrectiva"
                TargetControlID="hfDetalleAccionCorrectiva">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upDetalleAccionCorrectiva" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnDetalleAccionCorrectiva" runat="server" CssClass="Popup_frontground"
                        ScrollBars="Auto" Height="500">
                        <table align="center" class="tableControls">
                            <tr>
                                <td colspan="2" class="tdTitle">
                                    <h1>
                                        Acción Correctiva</h1>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label5" runat="server" Text="Descripción" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbDescripcionAccionCorrectiva" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label6" runat="server" Text="Responsable" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbNombreResponsableAccionCorrectiva" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label7" runat="server" Text="Fecha límite" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbFechaLimiteAccionCorrectiva" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label8" runat="server" Text="Fecha de ejecución" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbFechaRealizadoAccionCorrectiva" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label9" runat="server" Text="Observación" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbObservacionAccionCorrectiva" runat="server"></asp:Label>
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
                                    <asp:GridView ID="gvArchivosAccionCorrectiva" runat="server" AutoGenerateColumns="False"
                                        HorizontalAlign="Center" Width="863px" OnRowCommand="gvArchivosAccionCorrectiva_RowCommand"
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
                        <table class="tableControls" align="center">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btDetalleAccionCorrectivaCerrar" runat="server" Text="Cerrar" CssClass="submitButton"
                                        OnClick="btDetalleAccionCorrectivaCerrar_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="gvArchivosAccionCorrectiva" />
                </Triggers>
            </asp:UpdatePanel>
            <!-- Detalle Acción Correctiva !-->
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

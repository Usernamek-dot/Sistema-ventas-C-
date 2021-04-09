using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; //ejecutar comandos
using Microsoft.VisualBasic;


namespace VentasSharp2021
{
    public partial class frmProducto : Form


    {

        //lo que se define aqui es publico para todos los elementos de la clase
        //constructor que inicia obj de la clase
        MySqlConnection conexion = new MySqlConnection("server=localhost;database=almacenventas;uid=root;password=");
        //obtener parameto de sp
        object codigo,nombre,costo,existencia,unidadmedida,codigoUM;
        //variable
        Boolean switchValidarDatos;
        Boolean swtichEliminarProd;
        Boolean switchActualizar;

        public frmProducto()
        {
            InitializeComponent();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            //llamar funcion en formulario
            cargarProductos();
            cargarComboBoxUnidadMedida();
            ValidarDatos();


        }


        //funciones

        public void ValidarDatos()
        {
           

           
            if ((txtcodigo.Text == "")||(txtnombre.Text == "") || (txtcosto.Text == "") || (txtexistencia.Text == "") )
            {
               
                switchValidarDatos = true;
            }
            
            //cerra conexion
            conexion.Close();
        }

        public  void cargarComboBoxUnidadMedida()
        {
            // 

            //datable
            DataTable tablaUnidadMedida = new DataTable();


            //adaptador
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_ListarUnidadMedida", conexion);

            conexion.Open();

            //decir que tipo de comando se va a ejecutar
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;

            //ejecutar adaptador
            adaptador.Fill(tablaUnidadMedida);

            //validar si trae registros
            if (tablaUnidadMedida.Rows.Count > 0)
            {
                //imprimir en dgv
                txtunidadMedida.DataSource = tablaUnidadMedida;

                //ocultar unidad medida
                txtunidadMedida.DisplayMember = "descripcion";
                txtunidadMedida.ValueMember = "codigo";
            }
            else
            {
                MessageBox.Show("La tabla de unidad medida esta vacia", "Alerta", MessageBoxButtons.OK);
            }


            //cerra conexion
            conexion.Close();
        }

        public void cargarProductos()
        {
            //datable
            DataTable tabla = new DataTable();


            //adaptador
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_ListarProductos", conexion);

            conexion.Open();

            //decir que tipo de comando se va a ejecutar
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;

            //ejecutar adaptador
            adaptador.Fill(tabla);

            //validar si trae registros
            if (tabla.Rows.Count > 0) {
                //imprimir en dgv
                dgvProductos.DataSource = tabla;

                //ocultar unidad medida
                dgvProductos.Columns[4].Visible = false;
            } else {
                MessageBox.Show("La tabla de productos esta vacia","Alerta",MessageBoxButtons.OK);
            }


            //cerra conexion
            conexion.Close();      
        }
       
        public void mostrarUnProducto()
        {

            //datatable
            DataTable tablaBuscarUno = new DataTable();

            //adaptador
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_ListarUnProducto", conexion);

            conexion.Open();

            //decir que tipo de comando se va a ejecutar
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;



            //guardar parametro de sp
            codigo = Interaction.InputBox("Ingrese el codigo del producto");

            adaptador.SelectCommand.Parameters.Add("p_codigo", MySqlDbType.String).Value = codigo;

            //ejecutar adaptador
            adaptador.Fill(tablaBuscarUno);

            //validar si trae registros
            if (tablaBuscarUno.Rows.Count > 0)
            {
                //imprimir en dgv
                dgvProductos.DataSource = tablaBuscarUno;

                //ocultar unidad medida
                dgvProductos.Columns[4].Visible = false;

                //cerra conexion
                conexion.Close();

                swtichEliminarProd = true;
            }
            else
            {
                MessageBox.Show("No ha ingresado el codigo correctamente, favor verificar", "Alerta", MessageBoxButtons.OK , MessageBoxIcon.Information);
                //cerra conexion
                conexion.Close();
                //llamar funcion
                cargarProductos();
            }




            //cerra conexion
            conexion.Close();
        }

        public void limpiarForm()
        {
            txtcodigo.Clear();
            txtnombre.Clear();
            txtcosto.Clear();
            txtexistencia.Clear();

            if(txtcodigo.Enabled == false)
            {
                txtcodigo.Enabled = true;
            }
            txtcodigo.Focus();
        }


        //botones

        //btn insertar
        private void button4_Click(object sender, EventArgs e)
        {


            ValidarDatos();
            if (switchValidarDatos == true)
            {
                MessageBox.Show("Hay campos requeridos sin llenar", "Alerta", MessageBoxButtons.OK);
                switchValidarDatos = false;

            }
            else
            {

                try {
                    //datatable
                    DataTable tablaInsertarProd = new DataTable();

                    //adaptador
                    MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_InsertarProducto", conexion);
                    conexion.Open();


                    //cuadar parametros
                    codigo = txtcodigo.Text;
                    nombre = txtnombre.Text;
                    costo = txtcosto.Text;
                    existencia = txtexistencia.Text;
                    unidadmedida = txtunidadMedida.SelectedValue.ToString();


                    //decir que tipo de comando se va a ejecutar
                    adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adaptador.SelectCommand.Parameters.Add("p_codigo", MySqlDbType.String).Value = codigo;
                    adaptador.SelectCommand.Parameters.Add("p_nombre", MySqlDbType.String).Value = nombre;
                    adaptador.SelectCommand.Parameters.Add("p_costo", MySqlDbType.Int32).Value = costo;
                    adaptador.SelectCommand.Parameters.Add("p_existencia", MySqlDbType.Int32).Value = existencia;
                    adaptador.SelectCommand.Parameters.Add("p_unidadmedida", MySqlDbType.String).Value = unidadmedida;

                    adaptador.Fill(tablaInsertarProd);

                    MessageBox.Show("El Producto se envió existosamente.", "Alerta", MessageBoxButtons.OK);

                    limpiarForm();
                    conexion.Close();
                    cargarProductos();
                } catch (MySqlException Err){

                    if(Err.Number == 1062)
                    {
                        MessageBox.Show("El codigo Producto esta repetido.", "Alerta", MessageBoxButtons.OK);

                    }

                    conexion.Close();
                }
                catch (Exception Err2) {

                    MessageBox.Show("El codigo Producto esta repetido.", "Alerta", MessageBoxButtons.OK);

                        
                }






            }

        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {

            ValidarDatos();
            if (switchValidarDatos == true)
            {
                MessageBox.Show("Hay campos requeridos sin llenar", "Alerta", MessageBoxButtons.OK);
                switchValidarDatos = false;

            }
            else
            {
                try
                {
                    //datatable
                    DataTable tablaActualizarProd = new DataTable();

                    //adaptador
                    MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_ActualizarProducto", conexion);
                    conexion.Open();


                    //cuadar parametros
                    codigo = txtcodigo.Text;
                    nombre = txtnombre.Text;
                    costo = txtcosto.Text;
                    existencia = txtexistencia.Text;


                    if (switchActualizar == true)
                    {
                        unidadmedida = txtunidadMedida.SelectedValue.ToString();
                    }
                    else
                    {
                        unidadmedida = codigoUM;
                    }

                    switchActualizar = false;
                    unidadmedida = txtunidadMedida.SelectedValue.ToString();

                    //decir que tipo de comando se va a ejecutar
                    adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adaptador.SelectCommand.Parameters.Add("p_codigo", MySqlDbType.String).Value = codigo;
                    adaptador.SelectCommand.Parameters.Add("p_nombre", MySqlDbType.String).Value = nombre;
                    adaptador.SelectCommand.Parameters.Add("p_costo", MySqlDbType.Int32).Value = costo;
                    adaptador.SelectCommand.Parameters.Add("p_existencia", MySqlDbType.Int32).Value = existencia;
                    adaptador.SelectCommand.Parameters.Add("p_unidadmedida", MySqlDbType.String).Value = unidadmedida;

                    adaptador.Fill(tablaActualizarProd);

                    MessageBox.Show("El Producto se actualizó exitosamente.");

                    limpiarForm();
                    conexion.Close();
                    cargarProductos();
                }
                catch (MySqlException Err)
                {

                    if (Err.Number == 1062)
                    {
                        MessageBox.Show("El codigo Producto esta repetido.", "Alerta", MessageBoxButtons.OK);

                    }
                    else
                    {
                        MessageBox.Show("Se generó el siguiente error:" + Err.Message);

                    }

                    conexion.Close();
                }
                catch(Exception Err2)
                {
                    MessageBox.Show("Se generó el siguiente error:" + Err2.Message);
                }

            }




        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            mostrarUnProducto();
            if (swtichEliminarProd == true)
            {
                //variable de dialogo
                DialogResult respuesta = MessageBox.Show("Quiere eliminar el producto(si/no)", "Eliminar producto", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {

                    //datatable
                    DataTable tblEliminarProd = new DataTable();
                    //adaptador
                    MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_EliminarProducto", conexion);
                    try
                    {
                        conexion.Open();
                        //decir que tipo de comando se va a ejecutar
                        adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
                        //guardar parametro de sp
                        adaptador.SelectCommand.Parameters.Add("p_codigo", MySqlDbType.String).Value = codigo;
                        //ejecutar adaptador
                        adaptador.Fill(tblEliminarProd);
                        MessageBox.Show("Producto eliminado");
                        conexion.Close();
                    }
                    catch (MySqlException Err)
                    {
                        if (Err.Number == 1451)
                        {
                            MessageBox.Show("El producto no puede eliminarse ya que esta asociado a la venta.", "Alerta", MessageBoxButtons.OK);

                        }
                        else
                        {
                            MessageBox.Show(  "Se generó el siguiente error:" + Err.Message);

                        }
                        conexion.Close();
                    }
                }
                else
                {
                    MessageBox.Show("El producto no se eliminó.", "Alerta", MessageBoxButtons.OK);

                }
                cargarProductos();
                swtichEliminarProd = false;

            }

        }

        private void txtunidadMedida_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switchActualizar = true;
        }

        private void dgvProductos_DoubleClick(object sender, EventArgs e)
        {
            int fila = dgvProductos.CurrentRow.Index;
            txtcodigo.Text = dgvProductos.Rows[fila].Cells[0].Value.ToString();
            txtnombre.Text = dgvProductos.Rows[fila].Cells[1].Value.ToString();
            txtcosto.Text = dgvProductos.Rows[fila].Cells[2].Value.ToString();
            txtexistencia.Text = dgvProductos.Rows[fila].Cells[3].Value.ToString();

            codigoUM = dgvProductos.Rows[fila].Cells[4].Value.ToString();
            txtunidadMedida.Text = dgvProductos.Rows[fila].Cells[5].Value.ToString();
            txtcodigo.Enabled = false; 
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            mostrarUnProducto();

        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            //se llama la funcion para mostrar todos los productos de nuevo
            cargarProductos();
            limpiarForm();
        }
        

        
    }
}

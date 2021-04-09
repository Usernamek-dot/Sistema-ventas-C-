using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace VentasSharp2021
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }


        //boton aceptar
        private void button1_Click(object sender, EventArgs e)
        {
            //crear conecciones

            //constructor que inicia obj de la clase
            MySqlConnection con = new MySqlConnection("server=localhost;database=almacenventas;uid=root;password=");
            //comandos
            MySqlCommand comando = new MySqlCommand();
            MySqlDataReader registro;

            try
            {
                comando.Connection = con;
                con.Open();
                comando.CommandText = "SELECT *  FROM tblusuario WHERE usuario = '" + textBox2.Text + " ' AND clave = '" + textBox1.Text + "' ";
                registro = comando.ExecuteReader();

                //si hay algo enla variable..
                if (registro.Read())
                {
                    //cuadro de dialogo
                    MessageBox.Show("BIENVENIDO A LA APLICACION","Aceptado",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    //crear objto menu principa
                    frmMenuPrincipal MenuPrincipal = new frmMenuPrincipal();
                    //ocultar formlario actual
                    this.Hide();
                    //mostrar segundo form
                    MenuPrincipal.Show();
                }
                else
                {
                    //cuadro de dialogo
                    MessageBox.Show("NO PUEDE ACCEDER A LA APLICACION", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(MySqlException)
            {
                //cuadro de dialogo
                MessageBox.Show("Error de la conexion a la base de datos", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            //cerrar la conexion
            con.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

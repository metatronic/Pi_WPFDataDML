using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFDataDML
{
    /// <summary>
    /// Interaction logic for ContinentCountryState.xaml
    /// </summary>
    public partial class ContinentCountryState : Window
    {
        SqlConnection connection;
        ObservableCollection<Continent> continentList;
        ObservableCollection<Country> countryList;
        ObservableCollection<State> stateList;

        public ContinentCountryState()
        {
            InitializeComponent();
            continentList = new ObservableCollection<Continent>();
            countryList = new ObservableCollection<Country>();
            stateList = new ObservableCollection<State>();
        }

        class Continent
        {
            public int ContinentId { get; set; }
            public string ContinentName { get; set; }
        }
        class Country
        {
            public string CountryId { get; set; }
            public string CountryName { get; set; }
            public string Capital { get; set; }
            public int ContinentId { get; set; }
            public BitmapImage Flag { get; set; }

        }

        class State
        {
            public string StateId { get; set; }
            public string StateName { get; set; }
            public string Capital { get; set; }
        }

        void GetContinents()
        {
            try
            {
                string sql = "select * from continent";
                SqlCommand command = new SqlCommand(sql, connection);

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    Continent continent = new Continent()
                    {
                        ContinentId = Convert.ToInt32(reader["ContinentId"]),
                        ContinentName = reader["ContinentName"].ToString()
                    };
                    continentList.Add(continent);

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            ContinentCombo.ItemsSource = continentList;
            CountryCombo.ItemsSource = countryList;
            StateCombo.ItemsSource = stateList;
            GetContinents();
        }

        private void ContinentCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                countryList.Clear();
                int continentId = Convert.ToInt32((sender as ComboBox).SelectedValue);
                string sql = "select * from country where continentid = @conid";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("conid", continentId);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    Country country = new Country()
                    {
                        CountryId = reader["CountryId"].ToString(),
                        Capital = reader["Capital"].ToString(),
                        CountryName = reader["CountryName"].ToString()
                    };
                    Stream StreamObj = new MemoryStream((byte[])reader["Flag"]);
                    BitmapImage BitObj = new BitmapImage();
                    BitObj.BeginInit();
                    BitObj.StreamSource = StreamObj;
                    BitObj.EndInit();
                    country.Flag = BitObj;
                    countryList.Add(country);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void CountryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as ComboBox).SelectedValue != null)
                {
                    string countryId = (sender as ComboBox).SelectedValue.ToString();
                    CountryPanel.DataContext = countryList.Where(country => country.CountryId == countryId).Single();

                    stateList.Clear();

                    string sql = "select * from state where countryId = @conid";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("conid", countryId);
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        State state = new State()
                        {
                            StateName = reader["StateName"].ToString(),
                            Capital = reader["StateCapital"].ToString(),
                            StateId = reader["StateId"].ToString()
                        };
                        stateList.Add(state);
                    }
                }
                else
                {
                    CountryPanel.DataContext = null;
                    StatePanel.DataContext = null;
                    stateList.Clear();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void StateCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedValue != null)
            {
                string stateId = (sender as ComboBox).SelectedValue.ToString();
                StatePanel.DataContext = stateList.Where(state => state.StateId == stateId).Single();
            }
            else
            {
                StatePanel.DataContext = null;
            }
        }
    }
}

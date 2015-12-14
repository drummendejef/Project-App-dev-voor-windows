using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;
using BOBApp.ViewModels;
using Libraries.Models;
using Windows.UI.Xaml.Controls;

namespace BOBApp
{
    public class BaseViewModelLocator //Gaat de views aan de viewmodels koppelen. Moet aangeroepen worden (zie app.xaml)
    {
       
     
        public BaseViewModelLocator()
        {//Zorgen dat je geen harde verbinding hebt tussen 2 dingen, objecten injecteren in containers.

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);//In onze Ioc container zal een bepaalde view zitten en gekoppeld worden.


            //VMs registreren.
            SimpleIoc.Default.Register<MainViewVM>();
            SimpleIoc.Default.Register<BestemmingenVM>();
            SimpleIoc.Default.Register<FeestjesOverzichtVM>();
            SimpleIoc.Default.Register<LoginVM>();
            SimpleIoc.Default.Register<MijnRittenVM>();
            SimpleIoc.Default.Register<ProfielVM>();
            SimpleIoc.Default.Register<PuntenVM>();
            SimpleIoc.Default.Register<RegisterVM>();
            SimpleIoc.Default.Register<VindRitVM>();
            SimpleIoc.Default.Register<VindRitFilterVM>();
            SimpleIoc.Default.Register<VindRitChatVM>();
            SimpleIoc.Default.Register<ZoekVriendenVM>();




        }
        public MainViewVM MainViewVM
        {
            get { return ServiceLocator.Current.GetInstance<MainViewVM>(); }//Viewmodels opvragen
        }
        public BestemmingenVM BestemmingenVM
        {
            get { return ServiceLocator.Current.GetInstance<BestemmingenVM>(); }//Viewmodels opvragen
        }
        public FeestjesOverzichtVM FeestjesOverzichtVM
        {
            get { return ServiceLocator.Current.GetInstance<FeestjesOverzichtVM>(); }//Viewmodels opvragen
        }

        public LoginVM LoginVM
        {
            get { return ServiceLocator.Current.GetInstance<LoginVM>(); }//Viewmodels opvragen
        }

        public ProfielVM ProfielVM
        {
            get { return ServiceLocator.Current.GetInstance<ProfielVM>(); }//Viewmodels opvragen
        }

        public PuntenVM PuntenVM
        {
            get { return ServiceLocator.Current.GetInstance<PuntenVM>(); }//Viewmodels opvragen
        }

        public RegisterVM RegisterVM
        {
            get { return ServiceLocator.Current.GetInstance<RegisterVM>(); }//Viewmodels opvragen
        }

     
        public VindRitVM VindRitVM
        {
            get { return ServiceLocator.Current.GetInstance<VindRitVM>(); }//Viewmodels opvragen
        }

        public VindRitFilterVM VindRitFilterVM
        {
            get { return ServiceLocator.Current.GetInstance<VindRitFilterVM>(); }//Viewmodels opvragen
        }

        public VindRitChatVM VindRitChatVM
        {
            get { return ServiceLocator.Current.GetInstance<VindRitChatVM>(); }//Viewmodels opvragen
        }

        public ZoekVriendenVM ZoekVriendenVM
        {
            get { return ServiceLocator.Current.GetInstance<ZoekVriendenVM>(); }//Viewmodels opvragen
        }
        public MijnRittenVM MijnRittenVM
        {
            get { return ServiceLocator.Current.GetInstance<MijnRittenVM>(); }//Viewmodels opvragen
        }
    }
}

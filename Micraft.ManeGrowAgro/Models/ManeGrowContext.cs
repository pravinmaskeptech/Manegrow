using System.Data.Entity;

namespace Micraft.ManeGrowAgro.Models
{
    public class ManeGrowContext : DbContext
    {
        public ManeGrowContext() 
            : base("DefaultConnection")
        {
            //  this.Configuration.LazyLoadingEnabled = true;
            //this.Database.CommandTimeout = 300;
        }
       
       public virtual DbSet<CustomerType> CustomerTypes { get; set; }   
        public virtual DbSet<CustomerMaster> CustomerMasters { get; set; }
        public virtual DbSet<DepartmentMaster> DepartmentMasters { get; set; }
        public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }
        public virtual DbSet<HarvestingMaster> HarvestingMasters { get; set; }
        public virtual DbSet<ProductMaster> ProductMasters { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<PincodeMasters> PincodeMasters { get; set; }
        public virtual DbSet<RegionMaster> RegionMasters { get; set; }
        public virtual DbSet<States> States { get; set; }   
        public virtual DbSet<Zones> Zones { get; set; }
        public virtual DbSet<ZonesMaster> ZonesMaster { get; set; }

        public virtual DbSet<UoMeasure> UoMeasures { get; set; }    
        public virtual DbSet<EmpDesignation> EmpDesignations { get; set; }  
        public virtual DbSet<VehicleMaster> VehicleMasters { get; set; } 
        public  virtual DbSet<CutOffMasater> CutOffMasaters { get; set; }
        public virtual DbSet<VendorMaster> VendorMasters { get; set; }  
        public virtual DbSet<BankMaster> BankMasters { get; set; }  

        public virtual DbSet<TransMode> TransModes { get; set; }

        public virtual DbSet<MonthlyProdPlan> MonthlyProdPlan { get; set; }
        public virtual DbSet<WeeklyProdPlan> WeeklyProdPlan { get; set; }
        public virtual DbSet<RoomMaster> RoomMaster { get; set; }
        public virtual DbSet<DailyProdPlan> DailyProdPlan { get; set; }
        public virtual DbSet<Production> Production { get; set; }
        public virtual DbSet<ActualProduction> ActualProduction { get; set; }
        public virtual DbSet<MonthlyExcel> MonthlyExcel { get; set; }
        public virtual DbSet<CaretMaster> CaretMaster { get; set; }
        public virtual DbSet<AspNetRole> AspNetRole { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }

        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
            
        public virtual DbSet<MainOrder> MainOrder { get; set; }
        public virtual DbSet<Order> Order { get; set; }


        public virtual DbSet<QuotationDetails> QuotationDetails { get; set; }
        public virtual DbSet<QuotationMain> QuotationMain { get; set; }
        public virtual DbSet<SubRouteMain> SubRouteMain { get; set; } 
        public virtual DbSet<SubRouteDetails> SubRouteDetails { get; set; }


        public virtual DbSet<PackingDetails> PackingDetails { get; set; }
        public virtual DbSet<DriverMaster> DriverMaster { get; set; } 

        public virtual DbSet<VendorRouteAssignment> vendorRouteAssignment { get; set; } 

        public virtual DbSet<PackingHistory> packingHistory { get; set; }
        public virtual DbSet<DispatchDetails> dispatchDetails { get; set; } 

        public virtual DbSet<TrackingDetails> trackingDetails { get; set; }

        public virtual DbSet<VendorCityMaster> VendorCityMaster  { get; set; }

        public System.Data.Entity.DbSet<Micraft.ManeGrowAgro.Models.RouteMain> RouteMains { get; set; }
        public System.Data.Entity.DbSet<Micraft.ManeGrowAgro.Models.RouteDetails> RouteDetails { get; set; }

        public System.Data.Entity.DbSet<Micraft.ManeGrowAgro.Models.VehicleTypes> VehicleTypes { get; set; }
        public System.Data.Entity.DbSet<Micraft.ManeGrowAgro.Models.VendorQuotationMain> vendorQuotationMains { get; set; } 
        public System.Data.Entity.DbSet<Micraft.ManeGrowAgro.Models.VendorQuotationDetails> vendorQuotationDetails { get; set; }

        public System.Data.Entity.DbSet<Micraft.ManeGrowAgro.Models.VendorTypes> VendorTypes { get; set; }
        public System.Data.Entity.DbSet<Micraft.ManeGrowAgro.Models.DMSFile> DMSFile { get; set; }
        public System.Data.Entity.DbSet<Micraft.ManeGrowAgro.Models.ModeMaster> ModeMasters { get; set; }  


        public virtual DbSet<SubRouteCustomers> SubRouteCustomers { get; set; }
        public virtual DbSet<DispatchTransactions> DispatchTransactions { get; set; }
        public virtual DbSet<RouteDestinations> RouteDestinations { get; set; } 
        public virtual DbSet<ManifestGroup> ManifestGroups { get; set; }
        public virtual DbSet<ManifestMain> ManifestMains { get; set; } 
        public virtual DbSet<ManifestDetails> ManifestDetails { get; set; }
        public virtual DbSet<TableNumbring> TableNumbring { get; set; }
        public virtual DbSet<ExpenceTypes> ExpenceTypes { get; set; }
        public virtual DbSet<VendorExpence> VendorExpence  { get; set; }
        public virtual DbSet<ExpenceDetails> ExpenceDetails { get; set; }
        public virtual DbSet<DamageReasons> DamageReasons { get; set; }
        public virtual DbSet<PODUpdation> PODUpdations { get; set; }
        public virtual DbSet<ResaleTypeMaster> ResaleTypeMasters { get; set; }

        public virtual DbSet<RejectionSoldHistory> RejectionSoldHistory { get; set; }

        public virtual DbSet<CollectionDetails> CollectionDetails { get; set; }

        public virtual DbSet<CollectionMain> CollectionMains { get; set; }


        public virtual DbSet<PODNotUploadedVendorList> PODNotUploadedVendorList { get; set; } 
        public virtual DbSet<BoxTypeMaster> BoxTypeMaster { get; set; }



        //public virtual DbSet<RateMaster> RateMasters { get; set; } 












    }
}
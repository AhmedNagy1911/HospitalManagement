using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
    IdentityDbContext<ApplicationUser>(options), IUnitOfWork
{
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();
    public DbSet<PatientDoctor> PatientDoctors => Set<PatientDoctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Bed> Beds => Set<Bed>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionMedication> PrescriptionMedications => Set<PrescriptionMedication>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<MedicalReport> MedicalReports => Set<MedicalReport>();
    public DbSet<LabResult> LabResults => Set<LabResult>();
    public DbSet<RadiologyResult> RadiologyResults => Set<RadiologyResult>();
    public DbSet<GeneralReportDetail> GeneralReportDetails => Set<GeneralReportDetail>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();
    public DbSet<PatientDoctor> PatientDoctors => Set<PatientDoctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
using RVPark.Core.Interfaces;
using RVPark.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Application
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private IGenericRepository<AssetRequest> _AssetRequest;
        private IGenericRepository<Core.Models.File> _File;
        private IGenericRepository<Message> _Message;
        private IGenericRepository<Payment> _Payment;
        private IGenericRepository<Project> _Project;
        private IGenericRepository<ProjectFile> _ProjectFile;
        private IGenericRepository<ProjectProposal> _ProjectProposal;
        private IGenericRepository<ProjectProposalFile> _ProjectProposalFile;
        private IGenericRepository<ProjectTask> _ProjectTask;
        private IGenericRepository<ProjectUser> _ProjectUser;
        private IGenericRepository<ProjectUserHoursLog> _ProjectUserHoursLog;
        private IGenericRepository<ApplicationUser> _User;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public IGenericRepository<AssetRequest> AssetRequest
        {
            get
            {
                _AssetRequest ??= new GenericRepository<AssetRequest>(_db);
                return _AssetRequest;
            }
        }
        
        public IGenericRepository<Core.Models.File> File
        {
            get
            {
                _File ??= new GenericRepository<Core.Models.File>(_db);
                return _File;
            }
        }

        public IGenericRepository<Message> Message
        {
            get
            {
                _Message ??= new GenericRepository<Message>(_db);
                return _Message;
            }
        }
        public IGenericRepository<Payment> Payment
        {
            get
            {
                _Payment ??= new GenericRepository<Payment>(_db);
                return _Payment;
            }
        }

        public IGenericRepository<Project> Project
        {
            get
            {
                _Project ??= new GenericRepository<Project>(_db);
                return _Project;
            }
        }

        public IGenericRepository<ProjectFile> ProjectFile
        {
            get
            {
                _ProjectFile ??= new GenericRepository<ProjectFile>(_db);
                return _ProjectFile;
            }
        }

        public IGenericRepository<ProjectProposal> ProjectProposal
        {
            get
            {
                _ProjectProposal ??= new GenericRepository<ProjectProposal>(_db);
                return _ProjectProposal;
            }
        }

        public IGenericRepository<ProjectProposalFile> ProjectProposalFile
        {
            get
            {
                _ProjectProposalFile ??= new GenericRepository<ProjectProposalFile>(_db);
                return _ProjectProposalFile;
            }
        }

        public IGenericRepository<ProjectTask> ProjectTask
        {
            get
            {
                _ProjectTask ??= new GenericRepository<ProjectTask>(_db);
                return _ProjectTask;
            }
        }

        public IGenericRepository<ProjectUser> ProjectUser
        {
            get
            {
                _ProjectUser ??= new GenericRepository<ProjectUser>(_db);
                return _ProjectUser;
            }
        }

        public IGenericRepository<ProjectUserHoursLog> ProjectUserHoursLog
        {
            get
            {
                _ProjectUserHoursLog ??= new GenericRepository<ProjectUserHoursLog>(_db);
                return _ProjectUserHoursLog;
            }
        }

        public IGenericRepository<ApplicationUser> User
        {
            get
            {
                _User ??= new GenericRepository<ApplicationUser>(_db);
                return _User;
            }
        }
    }
}

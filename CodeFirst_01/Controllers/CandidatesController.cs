using CodeFirst_01.Models;
using CodeFirst_01.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst_01.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly CandidateDbContext _context;
        private readonly IWebHostEnvironment _web;
        public CandidatesController(CandidateDbContext _context, IWebHostEnvironment _web)
        {
            this._context = _context;
            this._web = _web;
        }
        public async Task< IActionResult> Index()
        {
            return View(await _context.Candidates.Include(c=>c.CandidateSkills).ThenInclude(s=>s.Skill).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }

        //public IActionResult AddNewSkill(int?id)
        //{
        //    ViewBag.skill = new SelectList(_context.Skills, "SkillId", "SkillName", id.ToString() ?? "");
        //    return PartialView("_addNewSkill");
        //}

        public IActionResult AddNewSkills(int? id)
        {
            ViewBag.skill = new SelectList(_context.Skills, "SkillId", "SkillName", id.ToString() ?? "");
            return PartialView("_addNewSkills");
        }

        [HttpPost]
        public  async Task< IActionResult> Create(CandidateVM vm,int[] skillId)
        {
            if (ModelState.IsValid)
            {
                Candidate candidate = new Candidate()
                {
                    CandidateName = vm.CandidateName,
                    DateOfBirth = vm.DateOfBirth,
                    Phone = vm.Phone,
                    Fresher = vm.Fresher
                };

                //Image
                var file = vm.ImagePath;
                string webroot = _web.WebRootPath;
                string folder = "Images";
                string imgFileName = Path.GetFileName(file.FileName);
                string fileToSave = Path.Combine(webroot, folder, imgFileName);
                if(file != null)
                {
                    using(var stream=new FileStream(fileToSave, FileMode.Create))
                    {
                        vm.ImagePath.CopyTo(stream);
                        candidate.Image = "/" + folder + "/" + imgFileName;
                    }
                }

                foreach (var item in skillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill()
                    {
                        Candidate = candidate,
                        CandidateId = candidate.CandidateId,
                        SkillId = item
                    };
                    _context.CandidateSkills.Add(candidateSkill);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            var candidate = _context.Candidates.FirstOrDefault(x => x.CandidateId == id);
            CandidateVM candidateVM = new CandidateVM()
            {
                CandidateId = candidate.CandidateId,
                CandidateName = candidate.CandidateName,
                DateOfBirth = candidate.DateOfBirth,
                Phone = candidate.Phone,
                Image = candidate.Image,
                Fresher = candidate.Fresher
            };

            var existSkill = _context.CandidateSkills.Where(x => x.CandidateId == id).ToList();
            foreach (var item in existSkill)
            {
                candidateVM.SkillList.Add(item.SkillId);
            }

            return View(candidateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CandidateVM vm, int[] skillId)
        {
            if (ModelState.IsValid)
            {
                Candidate candidate = new Candidate()
                {
                    CandidateId = vm.CandidateId,
                    CandidateName = vm.CandidateName,
                    DateOfBirth = vm.DateOfBirth,
                    Phone = vm.Phone,
                    Image = vm.Image,
                    Fresher = vm.Fresher
                };

                //image
                var file = vm.ImagePath;
                var existImg = vm.Image;

                if (file != null)
                {
                    string webroot = _web.WebRootPath;
                    string folder = "Images";
                    string imgFileName = Path.GetFileName(file.FileName);
                    string fileToSave = Path.Combine(webroot, folder, imgFileName);

                    using(var stream=new FileStream(fileToSave, FileMode.Create))
                    {
                        vm.ImagePath.CopyTo(stream);
                        candidate.Image = "/" + folder + "/" + imgFileName;
                    }
                }
                else
                {
                    candidate.Image = existImg;
                }

                var existSkill = _context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
                foreach (var item in existSkill)
                {
                    _context.CandidateSkills.Remove(item);
                }

                //Add

                foreach (var item in skillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill()
                    {
                        CandidateId = candidate.CandidateId,
                        SkillId=item,
                    };
                    _context.CandidateSkills.Add(candidateSkill);
                }
                _context.Update(candidate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            return View();
        }
        public IActionResult Delete(int ? id)
        {
            var candidate = _context.Candidates.FirstOrDefault(x => x.CandidateId == id);
            var existSkill = _context.CandidateSkills.Where(x => x.CandidateId == id).ToList();
            foreach(var item in existSkill)
            {
                _context.CandidateSkills.Remove(item);
            }
            _context.Remove(candidate);
            _context.SaveChanges();
            return RedirectToAction("Index");
          
        }
    }
}


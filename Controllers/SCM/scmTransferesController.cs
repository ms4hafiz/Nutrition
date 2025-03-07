﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.SCM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Navigations;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    public class scmTransfersController : Controller
    {
        private readonly WebNutContext _context;
        IHostingEnvironment hostingEnv;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmTransfersController(WebNutContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            this.hostingEnv = env;
        }
        public IActionResult Index()
        {
                return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";
            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Stock Detail", IconCss = "e-tab1" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Stock Transfer", IconCss = "e-tab2" }, Content = ViewBag.content2 });
            ViewBag.headeritems = headerItems;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = false;
                    ViewBag.gridDelete = false;

                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    ViewBag.gridAdd = true;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            if (id == null)
            {
                return NotFound();
            }

            var scmTransfers = _context.vscmDistributiontransfer.SingleOrDefault(m => m.id == id);
            if (scmTransfers == null)
            {
                return NotFound();
            }

            var warehouses = _context.scmWarehouses.Where(m => m.LevelId == 2).Select(m => new
            {
                WhId = m.WhId,
                WarehouseName = m.RegionsNav.RegionLong + '(' + m.ProvincesNav.ProvName + ") - " + m.ImplementerNav.ImpAcronym + " = " + m.Location
            }).ToList();

            ViewBag.WarehouseSource = warehouses;

            return View(scmTransfers);
        }
        public async Task<IActionResult> UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.vscmDistributiontransfer.ToList();
            if ((user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
            {
                data = data.Where(m => m.tenantId.Equals(user.TenantId)).ToList();
            }
            IEnumerable DataSource = data;
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<vscmDistributiontransfer>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        public async Task<IActionResult> TransferUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.scmTransfers.ToList();
            if ((user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
            {
                data = data.Where(m => m.tenantId.Equals(user.TenantId)).ToList();
            }
            IEnumerable DataSource = data;
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<scmTransfers>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmTransfers> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmTransfers item = new scmTransfers();
            if (item == null) { return BadRequest(); }

            item.ipdistributionId = int.Parse(value.Params["ID"].ToString());
            item.whId = value.Value.whId;
            item.quantitiy = value.Value.quantitiy;
            item.transferDate = value.Value.transferDate;
            item.tenantId = user.TenantId;
            item.userName = user.UserName;
            item.updateDate = DateTime.Now.Date;
            item.remarks = value.Value.remarks;

            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    return NoContent();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.Add(item);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<scmTransfers> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var item = _context.scmTransfers.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (item != null)
            {
                item.ipdistributionId = int.Parse(value.Params["ID"].ToString());
                item.whId = value.Value.whId;
                item.quantitiy = value.Value.quantitiy;
                item.transferDate = value.Value.transferDate;
                item.tenantId = user.TenantId;
                item.userName = user.UserName;
                item.updateDate = DateTime.Now.Date;
                item.remarks = value.Value.remarks;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(item).State = EntityState.Modified;

            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    return NoContent();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.Update(item);
                    _context.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        public async Task<IActionResult> Remove([FromBody]CRUDModel<scmTransfers> Value)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmTransfers item = _context.scmTransfers.Where(m => m.id.Equals(id)).FirstOrDefault();

                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    return NoContent();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.scmTransfers.Remove(item);
                    _context.SaveChanges();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }

        private bool Exists(int id)
        {
            return _context.scmTransfers.Any(e => e.id == id);
        }
    }
}
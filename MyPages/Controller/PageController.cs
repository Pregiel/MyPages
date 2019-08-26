using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyPages.Services;

namespace MyPages.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class PageController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPageService _pageService;

        public PageController(IUserService userService,
            IPageService pageService)
        {
            _userService = userService;
            _pageService = pageService;
        }

        // POST: api/Page/5/Relocate
        [HttpPost("{id}/relocate")]
        public async Task<IActionResult> ChangePosition(int id, [FromBody] string value)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var mainPage = await _pageService.GetByIdWithAll(id);
            if (mainPage == null)
                return NotFound();

            if (!_pageService.CheckAccess(mainPage, user))
                return Unauthorized();

            var itemsOrder = value.Split(";");
            if (itemsOrder.Count() != mainPage.Children.Count())
                return BadRequest();

            foreach (var order in itemsOrder)
            {
                var item = order.Split("=");

                var pageNo = int.Parse(item[0]);
                var pageId = int.Parse(item[1]);

                var page = await _pageService.GetByIdWithAll(pageId);
                page.OrdinalNumber = pageNo;
                await _pageService.Update(page);
            }

            return Ok();
        }

        // DELETE: api/Page/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var page = await _pageService.GetByIdWithAllParents(id);
            if (page == null)
                return NotFound();

            if (!_pageService.CheckAccess(page, user))
                return Unauthorized();

            await _pageService.Delete(id);

            return Ok();
        }



        // GET: api/Page/5/color
        [HttpGet("{id}/color")]
        public async Task<IActionResult> GetColor(int id)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var mainPage = await _pageService.GetByIdWithAll(id);
            if (mainPage == null)
                return NotFound();

            if (!_pageService.CheckAccess(mainPage, user))
                return Unauthorized();

            var page = await _pageService.GetById(id);
            return Ok(page.Color);
        }



        // POST: api/Page/5/color
        [HttpPost("{id}/color")]
        public async Task<IActionResult> SetColor(int id, [FromBody] string color)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var mainPage = await _pageService.GetByIdWithAll(id);
            if (mainPage == null)
                return NotFound();

            if (!_pageService.CheckAccess(mainPage, user))
                return Unauthorized();

            var page = await _pageService.GetById(id);
            page.Color = color;
            await _pageService.Update(page);

            return Ok();
        }

    }
}

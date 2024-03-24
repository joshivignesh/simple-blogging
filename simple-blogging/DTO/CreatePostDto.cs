using Microsoft.AspNetCore.Mvc.Rendering;

namespace simple_blogging.DTO
{
    public class CreatePostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}

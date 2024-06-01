using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm_shop
{
    public class Comments
    {
        public int id {  get; set; }
        public int materialId { get; set; }
        public int userId { get; set; }
        public string materialMark {  get; set; }
        public string dateOfWriting { get; set; }
        public string commentText { get; set; }
        public int quantityLikeDislike { get; set; }
        public string commentId { get; set; }

        public Comments()
        {

        }

        public Comments(int id, int materialId, int userId, string materialMark, string dateOfWriting, string commentText, int quantityLikeDislike, string commentId)
        {
            this.id = id;
            this.materialId = materialId;
            this.userId = userId;
            this.materialMark = materialMark;
            this.dateOfWriting = dateOfWriting;
            this.commentText = commentText;
            this.quantityLikeDislike = quantityLikeDislike;
            this.commentId = commentId;
        }
    }
}

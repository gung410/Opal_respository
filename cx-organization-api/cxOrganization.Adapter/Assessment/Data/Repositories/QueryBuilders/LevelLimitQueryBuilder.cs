using System.Collections.Generic;
using System.Linq;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Shared.Extensions;

namespace cxOrganization.Adapter.Assessment.Data.Repositories.QueryBuilders
{
    public class LevelLimitQueryBuilder
    {
        IQueryable<LevelLimitEntity> _query;
        public LevelLimitQueryBuilder(IQueryable<LevelLimitEntity> query)
        {
            _query = query;
        }
        public IQueryable<LevelLimitEntity> Build()
        {
            return _query;
        }
        public LevelLimitQueryBuilder FilterByActivityIds(List<int> activityIds)
        {
            if (activityIds.IsNotNullOrEmpty())
            {
                if (activityIds.Count == 1)
                {
                    int value = activityIds[0];
                    _query = _query.Where(t => t.LevelGroup.ActivityId == value);
                }
                else
                {
                    _query = _query.Where(t => activityIds.Contains(t.LevelGroup.ActivityId));
                }
            }

            return this;
        }
        public LevelLimitQueryBuilder FilterByLevelGroupIds(List<int> levelgroupIds)
        {
            if (levelgroupIds.IsNotNullOrEmpty())
            {
                if (levelgroupIds.Count == 1)
                {
                    int value = levelgroupIds[0];
                    _query = _query.Where(t => t.LevelGroupId == value);
                }
                else
                {
                    _query = _query.Where(t => levelgroupIds.Contains(t.LevelGroupId.Value));
                }
            }

            return this;
        }

        public LevelLimitQueryBuilder FilterByLevelLimitIds(List<int> levelLimitIds)
        {
            if (levelLimitIds.IsNotNullOrEmpty())
            {
                if (levelLimitIds.Count == 1)
                {
                    int value = levelLimitIds[0];
                    _query = _query.Where(t => t.LevelLimitId == value);
                }
                else
                {
                    _query = _query.Where(t => levelLimitIds.Contains(t.LevelLimitId));
                }
            }

            return this;
        }

        public LevelLimitQueryBuilder FilterByCategoryIds(List<int> categoryIds)
        {
            if (categoryIds.IsNotNullOrEmpty())
            {
                if (categoryIds.Count == 1)
                {
                    int value = categoryIds[0];
                    _query = _query.Where(t => t.CategoryId == value);
                }
                else
                {
                    _query = _query.Where(t => categoryIds.Contains(t.CategoryId.Value));
                }
            }

            return this;
        }

        public LevelLimitQueryBuilder FilterByQuestionIds(List<int> questionIds)
        {
            if (questionIds.IsNotNullOrEmpty())
            {
                if (questionIds.Count == 1)
                {
                    int value = questionIds[0];
                    _query = _query.Where(t => t.QuestionId == value);
                }
                else
                {
                    _query = _query.Where(t => questionIds.Contains(t.QuestionId.Value));
                }
            }

            return this;
        }

        public LevelLimitQueryBuilder FilterByItemIds(List<int> itemIds)
        {
            if (itemIds.IsNotNullOrEmpty())
            {
                if (itemIds.Count == 1)
                {
                    int value = itemIds[0];
                    _query = _query.Where(t => t.ItemId == value);
                }
                else
                {
                    _query = _query.Where(t => itemIds.Contains(t.ItemId.Value));
                }
            }

            return this;
        }

        public LevelLimitQueryBuilder FilterByLevelGroupTags(List<string> levelGroupTags)
        {
            if (levelGroupTags.IsNotNullOrEmpty())
            {
                if (levelGroupTags.Count == 1)
                {
                    string value = levelGroupTags[0];
                    _query = _query.Where(t => t.LevelGroup.Tag == value);
                }
                else
                {
                    _query = _query.Where(t => levelGroupTags.Contains(t.LevelGroup.Tag));
                }
            }

            return this;
        }
    }
}

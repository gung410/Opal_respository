using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microservice.Form.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Form.Common.Helpers
{
    public class FulltextSearchHelper
    {
        /// <summary>
        /// Filter by search text, support multiple string prop. inFullTextSearchProps must be a list of one level string prop.
        /// Ex: Input searchText: "abc def", inFullTextSearchProps: [p => p.PropA, p => p.PropB] will return query which is (p.PropA contains ("abc" AND "def") OR p.PropB contains ("abc" AND "def")).
        /// </summary>
        /// <typeparam name="T">Query item Type.</typeparam>
        /// <param name="query">Query to search on.</param>
        /// <param name="searchText">Search text.</param>
        /// <param name="inFullTextSearchProps">List of property expression to search on.</param>
        /// <returns>Filtered by seach text query.</returns>
        public static IQueryable<T> BySearchText<T>(IQueryable<T> query, string searchText, params Expression<Func<T, string>>[] inFullTextSearchProps)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return query;
            }

            var searchWords = BuildSearchWords(searchText);
            var fullTextSearchPropNames = inFullTextSearchProps.Where(p => p != null).Select(fullTextSearchOf => ExpressionHelper.GetPropertyName(fullTextSearchOf)).ToList();

            var searchedQuery = BuildSearchQuery(query, searchWords, fullTextSearchPropNames);

            return searchedQuery;
        }

        /// <summary>
        /// Filter by search words, support multiple prop.
        /// Ex: Input searchText: "abc def", filterBySingleWordPredicateBuilders: [searchWord => (searchWord => p.PropA.contains(searchWord)), searchWord => (searchWord => p.PropB.contains(searchWord))]
        /// will return query which is (p.PropA contains ("abc" AND "def") OR p.PropB contains ("abc" AND "def")).
        /// </summary>
        /// <typeparam name="T">Query item Type.</typeparam>
        /// <param name="query">Query to search on.</param>
        /// <param name="searchText">Search text.</param>
        /// <param name="filterBySingleWordPredicateBuilders">List of property expression to search on.</param>
        /// <returns>Filtered by seach text query.</returns>
        public static IQueryable<T> BySearchWords<T>(IQueryable<T> query, string searchText, params Func<string, Expression<Func<T, bool>>>[] filterBySingleWordPredicateBuilders)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return query;
            }

            var searchWords = BuildSearchWords(searchText);

            var searchedQuery = BuildSearchQuery(query, searchWords, filterBySingleWordPredicateBuilders.Where(p => p != null).ToArray());

            return searchedQuery;
        }

        /// <summary>
        /// Build query for all search prop. Example: Seach by PropA, PropB for text "hello word" will generate query with predicate:
        /// (propA.Contains("hello") AND propA.Contains("word")) OR (propB.Contains("hello") AND propB.Contains("word")).
        /// </summary>
        /// <typeparam name="T">Search item Type.</typeparam>
        /// <param name="query">The query to apply search filter.</param>
        /// <param name="searchWords">All search single word from a search text.</param>
        /// <param name="fullTextSearchPropNames">All search by property names, to search for searchWords match one of prop item in the list.</param>
        /// <returns>The filtered query.</returns>
        private static IQueryable<T> BuildSearchQuery<T>(IQueryable<T> query, List<string> searchWords, IEnumerable<string> fullTextSearchPropNames)
        {
            Expression<Func<T, bool>> totalPropsPredicate = null;

            foreach (var fullTextSearchPropName in fullTextSearchPropNames)
            {
                // Build predicate for a search prop. Example: Seach by PropA for text "hello word" will generate predicate: propA.Contains("hello") AND propA.Contains("word")
                Expression<Func<T, bool>> singlePropPredicate = null;
                foreach (var searchWord in searchWords)
                {
                    Expression<Func<T, bool>> singleWordSinglePropPredicate = r => EF.Functions.Contains(EF.Property<string>(r, fullTextSearchPropName), searchWord);

                    singlePropPredicate = singlePropPredicate == null ? singleWordSinglePropPredicate : singlePropPredicate.AndAlso(singleWordSinglePropPredicate);
                }

                totalPropsPredicate = totalPropsPredicate == null ? singlePropPredicate : totalPropsPredicate.Or(singlePropPredicate);
            }

            return query.Where(totalPropsPredicate);
        }

        private static IQueryable<T> BuildSearchQuery<T>(IQueryable<T> query, List<string> searchWords, params Func<string, Expression<Func<T, bool>>>[] filterBySingleWordPredicateBuilders)
        {
            Expression<Func<T, bool>> totalPropsPredicate = null;

            foreach (var filterBySearchTextWordPredicateBuilder in filterBySingleWordPredicateBuilders)
            {
                // Build predicate for a search prop. Example: Seach by PropA for text "hello word" will generate predicate: propA.Contains("hello") AND propA.Contains("word")
                Expression<Func<T, bool>> singlePropPredicate = null;
                foreach (var searchWord in searchWords)
                {
                    Expression<Func<T, bool>> singleWordSinglePropPredicate = filterBySearchTextWordPredicateBuilder(searchWord);

                    singlePropPredicate = singlePropPredicate == null ? singleWordSinglePropPredicate : singlePropPredicate.AndAlso(singleWordSinglePropPredicate);
                }

                totalPropsPredicate = totalPropsPredicate == null ? singlePropPredicate : totalPropsPredicate.Or(singlePropPredicate);
            }

            return query.Where(totalPropsPredicate);
        }

        private static List<string> BuildSearchWords(string searchText)
        {
            // Remove special not supported character for full text search
            var removedSpecialCharactersSearchText = searchText.Replace("\"", " ").Replace("~", " ").Replace("[", " ").Replace("]", " ").Replace("(", " ").Replace(")", " ").Replace("!", " ");

            var searchWords = removedSpecialCharactersSearchText.Split(" ").Where(p => !string.IsNullOrWhiteSpace(p)).ToList();

            return searchWords;
        }
    }
}

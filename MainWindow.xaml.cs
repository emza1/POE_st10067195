using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Skamza
{
    public partial class MainWindow : Window
    {
        private List<Recipe> recipes;
        private Recipe selectedRecipe;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize recipe list and set data context
            recipes = new List<Recipe>();
            recipeListBox.ItemsSource = recipes;
        }

        private void RecipeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update selected recipe
            selectedRecipe = recipeListBox.SelectedItem as Recipe;

            // Update recipe details section
            if (selectedRecipe != null)
            {
                recipeNameTextBox.Text = selectedRecipe.Name;
                ingredientsListView.ItemsSource = selectedRecipe.Ingredients;
                totalCaloriesTextBlock.Content = $"Total Calories: {selectedRecipe.GetTotalCalories()}";
            }
            else
            {
                // Clear recipe details if no recipe is selected
                recipeNameTextBox.Text = string.Empty;
                ingredientsListView.ItemsSource = null;
                totalCaloriesTextBlock.Content = string.Empty;
            }
        }


        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            string recipeName = recipeNameTextBox.Text;


            if (!string.IsNullOrEmpty(recipeName))
            {
                Recipe newRecipe = new Recipe(recipeName);
                recipes.Add(newRecipe);
                recipeListBox.ItemsSource = recipes;

                // Clear recipe name input
                recipeNameTextBox.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Please enter a recipe name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string ingredientName = ingredientNameTextBox.Text.Trim();
            string caloriesText = caloriesTextBox.Text.Trim();
            string foodGroup = foodGroupTextBox.Text.Trim();

            if (string.IsNullOrEmpty(ingredientName) || string.IsNullOrEmpty(caloriesText) || string.IsNullOrEmpty(foodGroup))
            {
                MessageBox.Show("Please enter all ingredient details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(caloriesText, out int calories))
            {
                MessageBox.Show("Calories should be a valid integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if the maximum number of categories has been reached
            if (selectedRecipe.Ingredients.Count >= 300)
            {
                MessageBox.Show("Maximum number of categories has been reached. 300", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create new ingredient
            Ingredient newIngredient = new Ingredient(ingredientName, calories, foodGroup);

            // Add new ingredient to the selected recipe
            selectedRecipe.Ingredients.Add(newIngredient);

            // Refresh ingredients ListView
            ingredientsListView.Items.Refresh();

            // Update total calories
            totalCaloriesTextBlock.Content = $"Total Calories: {selectedRecipe.GetTotalCalories()}";

            // Check if the maximum number of calories has been reached
            int maxCalories = 300; // Set the maximum number of calories here
            if (selectedRecipe.GetTotalCalories() >= maxCalories)
            {
                maxCaloriesStatusTextBlock.Text = "Maximum Calories Reached: Yes";
            }
            else
            {
                maxCaloriesStatusTextBlock.Text = "Maximum Calories Reached: No";
            }


            // Clear ingredient inputs
            ingredientNameTextBox.Text = string.Empty;
            caloriesTextBox.Text = string.Empty;
            foodGroupTextBox.Text = string.Empty;
        }




        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ingredientNameFilter = filterIngredientNameTextBox.Text.Trim();
            string maxCaloriesFilter = filterMaxCaloriesTextBox.Text.Trim();

            // Apply filtering based on ingredient name and maximum calories
            List<Recipe> filteredRecipes = recipes.Where(recipe =>
            {
                bool nameMatch = string.IsNullOrEmpty(ingredientNameFilter) || recipe.Ingredients.Any(ingredient =>
                    ingredient.Name.Contains(ingredientNameFilter, StringComparison.OrdinalIgnoreCase));

                bool caloriesMatch = string.IsNullOrEmpty(maxCaloriesFilter) || recipe.GetTotalCalories() <= int.Parse(maxCaloriesFilter);

                return nameMatch && caloriesMatch;
            }).ToList();

            // Update the recipe list
            recipeListBox.ItemsSource = filteredRecipes;
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedFoodGroup = filterFoodGroupComboBox.SelectedItem as ComboBoxItem;

            if (selectedFoodGroup != null)
            {
                string foodGroupFilter = selectedFoodGroup.Content.ToString();

                // Apply filtering based on the selected food group
                List<Recipe> filteredRecipes = recipes.Where(recipe =>
                    recipe.Ingredients.Any(ingredient => ingredient.FoodGroup == foodGroupFilter)).ToList();

                // Update the recipe list
                recipeListBox.ItemsSource = filteredRecipes;
            }
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset the filter text boxes and combo box
            filterIngredientNameTextBox.Text = string.Empty;
            filterMaxCaloriesTextBox.Text = string.Empty;
            filterFoodGroupComboBox.SelectedIndex = -1;

            // Reset the recipe list to show all recipes
            recipeListBox.ItemsSource = recipes;
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset the filter text boxes and combo box
            filterIngredientNameTextBox.Text = string.Empty;
            filterMaxCaloriesTextBox.Text = string.Empty;
            filterFoodGroupComboBox.SelectedIndex = -1;

            maxCaloriesStatusTextBlock.Text = string.Empty;

            // Reset the recipe list to show all recipes
            recipeListBox.ItemsSource = recipes;
        }
    }
}
public class Recipe
{
    public string Name { get; set; }
    public List<Ingredient> Ingredients { get; set; }

    public Recipe(string name)
    {
        Name = name;
        Ingredients = new List<Ingredient>();
    }

    public int GetTotalCalories()
    {
        int totalCalories = 0;
        foreach (Ingredient ingredient in Ingredients)
        {
            totalCalories += ingredient.Calories;
        }
        return totalCalories;
    }
}


public class Ingredient
{
    public string Name { get; set; }
    public int Calories { get; set; }
    public string FoodGroup { get; set; }

    public Ingredient(string name, int calories, string foodGroup)
    {
        Name = name;
        Calories = calories;
        FoodGroup = foodGroup;
    }
}


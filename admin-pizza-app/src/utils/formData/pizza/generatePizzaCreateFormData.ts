import { IPizzaCreate } from "interfaces/pizza.ts";

export const generatePizzaCreateFormData = (pizza: IPizzaCreate): FormData => {
  const formData = new FormData();

  formData.append("Name", pizza.name);
  formData.append("Description", pizza.description);
  formData.append("CategoryId", pizza.categoryId);

  if (pizza.ingredientIds) {
    Array.from(pizza.ingredientIds).forEach((ingredient) => formData.append("IngredientIds", ingredient.toString()));
  }

  if (pizza.photos) {
    Array.from(pizza.photos).forEach((photo) => formData.append("Photos", photo));
  }

  if (pizza.sizes) {
    pizza.sizes.forEach((size, index) => {
      formData.append(`Sizes[${index}].sizeId`, size.sizeId.toString());
      formData.append(`Sizes[${index}].price`, size.price.toString());
    });
  }

  return formData;
};

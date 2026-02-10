const houses = [
  "House1.glb",
  "House2.glb",
  "Two story house1.glb",
  "Two story house2.glb",
  "Two story house3.glb",
  "Two story house4.glb",
  "Two story house5.glb",
  "Two story house6.glb",
];

export function buildHouse() {
  const randomHouseIndex = Math.floor(Math.random() * houses.length);
  const randomHouseString = houses[randomHouseIndex];

  const houseGroup = new THREE.Group();
  const gltfLoader = new THREE.GLTFLoader();

  gltfLoader.load("Suburban Houses Pack-glb/" + randomHouseString, (gltf) => {
    const houseModel = gltf.scene;

    houseModel.traverse((child) => {
      if (child.isMesh) {
        child.castShadow = true;
      }
    });

    houseGroup.add(houseModel);
  });

  return houseGroup;
}

export function buildFireHydrant() {
  const hydrantGroup = new THREE.Group();

  const redMaterial = new THREE.MeshStandardMaterial({
    color: 0xaa0000,
    roughness: 0.6,
  });
  const metalMaterial = new THREE.MeshStandardMaterial({
    color: 0x333333,
    metalness: 0.8,
    roughness: 0.2,
  });

  const bodyGeometry = new THREE.CylinderGeometry(0.5, 0.5, 1.2, 32);
  const body = new THREE.Mesh(bodyGeometry, redMaterial);
  body.castShadow = true;
  body.receiveShadow = true;
  hydrantGroup.add(body);

  const topGeometry = new THREE.SphereGeometry(
    0.5,
    32,
    32,
    0,
    Math.PI * 2,
    0,
    Math.PI / 2,
  );
  const top = new THREE.Mesh(topGeometry, redMaterial);
  top.position.y = 0.6;
  top.castShadow = true;
  hydrantGroup.add(top);

  const nutGeometry = new THREE.CylinderGeometry(0.15, 0.15, 0.2, 6);
  const nut = new THREE.Mesh(nutGeometry, redMaterial);
  nut.position.y = 1.1;
  nut.castShadow = true;
  hydrantGroup.add(nut);

  const baseGeometry = new THREE.CylinderGeometry(0.65, 0.65, 0.2, 32);
  const base = new THREE.Mesh(baseGeometry, redMaterial);
  base.position.y = -0.6;
  base.castShadow = true;
  base.receiveShadow = true;
  hydrantGroup.add(base);

  const waterGeometry = new THREE.CylinderGeometry(0.2, 0.2, 0.4, 32);
  const water = new THREE.Mesh(waterGeometry, metalMaterial);
  water.rotation.x = Math.PI / 2;
  water.position.set(0, 0.2, 0.4);
  water.castShadow = true;
  hydrantGroup.add(water);

  hydrantGroup.scale.set(0.08, 0.08, 0.08);

  return hydrantGroup;
}

export function buildStreet(length) {
  const streetGroup = new THREE.Group();
  const material = new THREE.MeshStandardMaterial({ color: "gray" });

  const streetGeom = new THREE.BoxGeometry(length, 1, 0.01);
  const street = new THREE.Mesh(streetGeom, material);
  street.rotation.x = -Math.PI / 2;
  street.receiveShadow = true;
  streetGroup.add(street);

  const drivewayGeom = new THREE.BoxGeometry(5, 1, 0.01);
  const driveway = new THREE.Mesh(drivewayGeom, material);
  driveway.rotation.x = -Math.PI / 2;
  driveway.position.y = 1;
  driveway.receiveShadow = true;
  streetGroup.add(driveway);

  return street;
}

export function buildTree() {
  const treeGroup = new THREE.Group();
  const gltfLoader = new THREE.GLTFLoader();

  gltfLoader.load("City Pack-glb/Tree.glb", (gltf) => {
    const tree = gltf.scene;

    tree.traverse((child) => {
      if (child.isMesh) {
        child.castShadow = true;
      }
    });

    treeGroup.add(tree);
  });

  treeGroup.scale.set(0.002, 0.002, 0.002);
  return treeGroup;
}